<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:rqi="http://schemas.datacontract.org/2004/07/MvcRQ.Models"
	xmlns:b="http://schemas.datacontract.org/2004/07/RQLib.RQKos.Classifications"
	xmlns:a="http://schemas.datacontract.org/2004/07/RQLib.RQQueryResult.RQDescriptionElements"
	xmlns:dc="http://purl.org/dc/elements/1.1/"
	xmlns:srw_dc="info:srw/schema/1/dc-v1.1"  
	xmlns:oai_dc="http://www.openarchives.org/OAI/2.0/oai_dc/"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  exclude-result-prefixes="rqi b a msxsl">
  
  <!-- 
This stylesheet transforms RQIntern (rqi) version 0.1 records and collections of records to simple Dublin Core (DC) records 
		
The stylesheet will transform a collection of RQIntern (rqi) 0.1 records into simple Dublin Core (DC)
as expressed by the SRU DC schema <http://www.loc.gov/standards/sru/dc-schema.xsd>

The stylesheet will transform a single RQIntern (rqi) 0.1 record into simple Dublin Core (DC)
as expressed by the OAI DC schema <http://www.openarchives.org/OAI/2.0/oai_dc.xsd>
		
This stylesheet makes the following decisions in its interpretation of the RQIntern (rqi) to simple DC mapping: 
	
When the roleTerm value associated with a name is creator, then name maps to dc:creator
When there is no roleTerm value associated with name, or the roleTerm value associated with name is a value other than creator, then name maps to dc:contributor
Start and end dates are presented as span dates in dc:date and in dc:coverage
When the first subelement in a subject wrapper is topic, subject subelements are strung together in dc:subject with hyphens separating them
Some subject subelements, i.e., geographic, temporal, hierarchicalGeographic, and cartographics, are also parsed into dc:coverage
The subject subelement geographicCode is dropped in the transform


Revision 0.0	yyyy-mm-dd <jbunzel@riquest.de>
	
Version 0.1	2010-07-12 Jürgen Bunzel <jbunzel@riquest.de>

-->
  
	<xsl:output method="xml" indent="yes"/>
	
	<xsl:param name="interface" select="'UNAPI'"/>
	
	<xsl:template name="ParseSubFields">
		<xsl:param name="FieldSubString"/>
		
		<xsl:variable name="Head" select="normalize-space(substring-before($FieldSubString,';'))"/>
		<xsl:if test="$Head">
			<xsl:element name="SubFields">
				<xsl:call-template name="escape-apos">
					<xsl:with-param name="string" select="$Head"/>
				</xsl:call-template>
			</xsl:element>
		</xsl:if>
		<xsl:variable name="Tail" select="normalize-space(substring-after($FieldSubString,';'))"/>
		<xsl:if test="$Tail">
			<xsl:call-template name="ParseSubFields">
				<xsl:with-param name="FieldSubString" select="$Tail"/>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	
	
	<xsl:template match="/">
		<xsl:choose>
			<xsl:when test="//rqi:RQItemModel">
				<xsl:choose>
					<xsl:when test="$interface='SRU'">
						<xsl:element name="records">
							<xsl:apply-templates/>
							<xsl:for-each select="//rqi:RQItemModel/rqi:RQItems/rqi:RQItems/rqi:RQItem">
								<xsl:element name="record">
									<xsl:element name="recordSchema">
										<xsl:value-of
											select="'info:srw/schema/1/dc-schema http://www.loc.gov/standards/sru/dc-schema.xsd'"
										/>
									</xsl:element>
									<xsl:element name="recordPacking">
										<xsl:value-of select="'xml'"/>
									</xsl:element>
									<xsl:element name="recordData">
										<srw_dc:dc
											xsi:schemaLocation="info:srw/schema/1/dc-schema http://www.loc.gov/standards/sru/dc-schema.xsd">
											<xsl:call-template name="DCRecord"/>
										</srw_dc:dc>
									</xsl:element>
								</xsl:element>
							</xsl:for-each>
						</xsl:element>
					</xsl:when>
					<xsl:otherwise>
						<srw_dc:dcCollection
							xsi:schemaLocation="info:srw/schema/1/dc-schema http://www.loc.gov/standards/sru/dc-schema.xsd">
							<xsl:apply-templates/>
							<xsl:for-each select="//rqi:RQItemModel/rqi:RQItems/rqi:RQItems/rqi:RQItem">
								<srw_dc:dc
									xsi:schemaLocation="info:srw/schema/1/dc-schema http://www.loc.gov/standards/sru/dc-schema.xsd">
									<xsl:call-template name="DCRecord"/>
								</srw_dc:dc>
							</xsl:for-each>
						</srw_dc:dcCollection>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:for-each select="rqi:RQItem">
					<oai_dc:dc
						xsi:schemaLocation="http://www.openarchives.org/OAI/2.0/oai_dc/ http://www.openarchives.org/OAI/2.0/oai_dc.xsd">
						<xsl:call-template name="DCRecord"/>
					</oai_dc:dc>
				</xsl:for-each>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	
	<xsl:template name="DCRecord">
		<xsl:if test="rqi:Title">
			<dc:title>
				<xsl:value-of select="rqi:Title"/>
			</dc:title>
		</xsl:if>
		<xsl:if test="rqi:Authors">
			<xsl:variable name="Subfields">
				<xsl:call-template name="ParseSubFields">
					<xsl:with-param name="FieldSubString" select="concat(rqi:Authors,';')"/>
				</xsl:call-template>
			</xsl:variable>
			
			<xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
				<xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
					<dc:creator>
						<xsl:value-of select="."/>
					</dc:creator>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
		<xsl:if test="rqi:Subjects | rqi:IndexTerms | rqi:AboutPersons">
			<!-- Classification not covered -->
			<xsl:variable name="Subfields">
				<xsl:call-template name="ParseSubFields">
					<xsl:with-param name="FieldSubString"
						select="concat(concat(rqi:Subjects,';'),concat(rqi:IndexTerms,';'),concat(rqi:AboutPersons,';'))"
					/>
				</xsl:call-template>
			</xsl:variable>
			
			<xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
				<xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
					<dc:subject>
						<xsl:value-of select="."/>
					</dc:subject>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
		<xsl:if test="rqi:Abstract">
			<xsl:if test="not(contains(rqi:Abstract,'$$'))">
				<dc:description>
					<xsl:value-of select="rqi:Abstract"/>
				</dc:description>
			</xsl:if>
		</xsl:if>
		<xsl:if test="rqi:Publisher">
			<xsl:variable name="Subfields">
				<xsl:call-template name="ParseSubFields">
					<xsl:with-param name="FieldSubString" select="concat(rqi:Publisher,';')"/>
				</xsl:call-template>
			</xsl:variable>
			
			<xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
				<xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
					<dc:publisher>
						<xsl:value-of select="."/>
					</dc:publisher>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
		<xsl:if test="rqi:Authors | rqi:Institutions">
			<xsl:variable name="Subfields">
				<xsl:call-template name="ParseSubFields">
					<xsl:with-param name="FieldSubString"
						select="concat(concat(rqi:Institutions,';'),concat(rqi:Authors,';'))"/>
				</xsl:call-template>
			</xsl:variable>
			
			<xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
				<xsl:if test="contains(.,'{') and not(contains(.,'=>'))">
					<dc:contributor>
						<xsl:value-of select="."/>
					</dc:contributor>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
		<xsl:if test="rqi:PublTime">
			<!-- CreateTime not covered -->
			<xsl:variable name="Subfields">
				<xsl:call-template name="ParseSubFields">
					<xsl:with-param name="FieldSubString" select="concat(rqi:PublTime,';')"/>
				</xsl:call-template>
			</xsl:variable>
			
			<xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
				<xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
					<dc:date>
						<xsl:value-of select="."/>
					</dc:date>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
		<xsl:if test="rqi:DocTypeName | rqi:WorkType">
			<xsl:variable name="Subfields">
				<xsl:call-template name="ParseSubFields">
					<xsl:with-param name="FieldSubString"
						select="concat(concat(rqi:DocTypeName,';'),concat(rqi:WorkType,';'))"/>
				</xsl:call-template>
			</xsl:variable>
			
			<xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
				<xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
					<dc:type>
						<xsl:value-of select="."/>
					</dc:type>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
		<xsl:if test="rqi:Pages">
			<dc:format>
				<xsl:value-of select="rqi:Pages"/>
			</dc:format>
		</xsl:if>
		<xsl:if test="rqi:ISDN | rqi:Coden">
			<xsl:variable name="Subfields">
				<xsl:call-template name="ParseSubFields">
					<xsl:with-param name="FieldSubString"
						select="concat(concat(rqi:ISDN,';'),concat(rqi:Coden,';'))"/>
				</xsl:call-template>
			</xsl:variable>
			
			<xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
				<xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
					<dc:identifier>
						<xsl:value-of select="."/>
					</dc:identifier>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
		<xsl:if test="rqi:Source">
			<xsl:variable name="Subfields">
				<xsl:call-template name="ParseSubFields">
					<xsl:with-param name="FieldSubString" select="concat(rqi:Source,';')"/>
				</xsl:call-template>
			</xsl:variable>
			
			<xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
				<xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
					<dc:source>
						<xsl:value-of select="."/>
					</dc:source>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
		<xsl:if test="rqi:Language">
			<xsl:variable name="Subfields">
				<xsl:call-template name="ParseSubFields">
					<xsl:with-param name="FieldSubString" select="concat(rqi:Language,';')"/>
				</xsl:call-template>
			</xsl:variable>
			
			<xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
				<xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
					<dc:language>
						<xsl:value-of select="."/>
					</dc:language>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
		<xsl:if test="rqi:Series">
			<xsl:variable name="Subfields">
				<xsl:call-template name="ParseSubFields">
					<xsl:with-param name="FieldSubString" select="concat(rqi:Series,';')"/>
				</xsl:call-template>
			</xsl:variable>
			
			<xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
				<xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
					<dc:relation>
						<xsl:value-of select="."/>
					</dc:relation>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
		<xsl:if test="rqi:AboutLocation | rqi:AboutTime">
			<xsl:variable name="Subfields">
				<xsl:call-template name="ParseSubFields">
					<xsl:with-param name="FieldSubString"
						select="concat(concat(rqi:AboutLocation,';'),concat(rqi:AboutTime,';'))"/>
				</xsl:call-template>
			</xsl:variable>
			
			<xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
				<xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
					<dc:coverage>
						<xsl:value-of select="."/>
					</dc:coverage>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
		<xsl:if test="rqi:RQRights">
			<dc:rights>
				<xsl:value-of select="rqi:RQRights"/>
			</dc:rights>
		</xsl:if>
	</xsl:template>
	
	<!-- suppress all else:-->
	<xsl:template match="*"> </xsl:template>
	
	<!-- template provided with 2 typos by Jeni Tennison, 25 Feb 2001 -->
	<xsl:template name="escape-apos">
		<xsl:param name="string"/>
		
		<xsl:value-of select="$string" disable-output-escaping="no"/>
	</xsl:template>
	
</xsl:stylesheet>
	