<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:rqi="http://schemas.datacontract.org/2004/07/MvcRQ.Models"
	xmlns:b="http://schemas.datacontract.org/2004/07/RQLib.RQKos.Classifications"
	xmlns:a="http://schemas.datacontract.org/2004/07/RQLib.RQQueryResult.RQDescriptionElements"
	xmlns:dc="http://purl.org/dc/elements/1.1/"
	xmlns:rq="http://www.riquest.de/rq"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  exclude-result-prefixes="rqi b a msxsl">
<!-- 
This stylesheet transforms RQIntern (rqi) version 0.1 records and collections of records to RiQuest proprietary exchange format (rq) records 
		
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
										<xsl:value-of select="'http://www.riquest.de/rq http://www.riquest.de/rq/rq-schema.xsd'" />
									</xsl:element>
									<xsl:element name="recordPacking">
										<xsl:value-of select="'xml'"/>
									</xsl:element>
									<xsl:element name="recordData">
										<rq:RQItemSet xsi:schemaLocation="http://www.riquest.de/rq http://www.riquest.de/rq/rq-schema.xsd">
											<xsl:call-template name="RQRecord"/>
										</rq:RQItemSet>
									</xsl:element>
								</xsl:element>
							</xsl:for-each>
						</xsl:element>
					</xsl:when>
					<xsl:otherwise>
						<rq:RQItemSet	xsi:schemaLocation="http://www.riquest.de/rq http://www.riquest.de/rq/rq-schema.xsd">
							<xsl:apply-templates/>
							<xsl:for-each select="//rqi:RQItemModel/rqi:RQItems/rqi:RQItems/rqi:RQItem">
								<rq:RQItem xsi:schemaLocation="http://www.riquest.de/rq http://www.riquest.de/rq/rq-schema.xsd">
									<xsl:call-template name="RQRecord"/>
								</rq:RQItem>
							</xsl:for-each>
						</rq:RQItemSet>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:for-each select="rqi:RQItem">
					<rq:RQItem xsi:schemaLocation="http://www.riquest.de/rq http://www.riquest.de/rq/rq-schema.xsd">
						<xsl:call-template name="RQRecord"/>
					</rq:RQItem>
				</xsl:for-each>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	
	<xsl:template name="RQRecord">
    <xsl:if test="rqi:DocNo != ''">
      <rq:DocNo>
        <xsl:value-of select="rqi:DocNo"/>
      </rq:DocNo>
    </xsl:if>
    <xsl:if test="rqi:Title != ''">
			<rq:Title>
				<xsl:value-of select="rqi:Title"/>
			</rq:Title>
		</xsl:if>
		<xsl:if test="rqi:Authors != ''">
      <rq:Authors>
        <xsl:value-of select="rqi:Authors"/>
      </rq:Authors>
    </xsl:if>
    <xsl:if test="rqi:Institutions != ''">
      <rq:Institutions>
        <xsl:value-of select="rqi:Institutions"/>
      </rq:Institutions>
    </xsl:if>
    <xsl:if test="rqi:Source != ''">
      <rq:Source>
        <xsl:value-of select="rqi:Source"/>
      </rq:Source>
    </xsl:if>
    <xsl:if test="rqi:Edition != ''">
      <rq:Edition>
        <xsl:value-of select="rqi:Edition"/>
      </rq:Edition>
    </xsl:if>
    <xsl:if test="rqi:Volume != ''">
      <rq:Volume>
        <xsl:value-of select="rqi:Volume"/>
      </rq:Volume>
    </xsl:if>
    <xsl:if test="rqi:Issue != ''">
      <rq:Issue>
        <xsl:value-of select="rqi:Issue"/>
      </rq:Issue>
    </xsl:if>
    <xsl:if test="rqi:Locality != ''">
      <rq:Locality>
        <xsl:value-of select="rqi:Locality"/>
      </rq:Locality>
    </xsl:if>
    <xsl:if test="rqi:Publisher != ''">
      <rq:Publisher>
        <xsl:value-of select="rqi:Publisher"/>
      </rq:Publisher>
    </xsl:if>
    <xsl:if test="rqi:PublTime != ''">
      <rq:PublTime>
        <xsl:value-of select="rqi:PublTime"/>
      </rq:PublTime>
    </xsl:if>
    <xsl:if test="rqi:Pages != ''">
      <rq:Pages>
        <xsl:value-of select="rqi:Pages"/>
      </rq:Pages>
    </xsl:if>
    <xsl:if test="rqi:Series != ''">
      <rq:Series>
        <xsl:value-of select="rqi:Series"/>
      </rq:Series>
    </xsl:if>
    <xsl:if test="rqi:ISDN != ''">
      <rq:ISDN>
        <xsl:value-of select="rqi:ISDN"/>
      </rq:ISDN>
    </xsl:if>
    <xsl:if test="rqi:Coden != ''">
      <rq:Coden>
        <xsl:value-of select="rqi:Coden"/>
      </rq:Coden>
    </xsl:if>
    <xsl:if test="rqi:Language != ''">
      <rq:Language>
        <xsl:value-of select="rqi:Language"/>
      </rq:Language>
    </xsl:if>
    <xsl:if test="rqi:DocTypeCode != ''">
      <rq:DocTypeCode>
        <xsl:value-of select="rqi:DocTypeCode"/>
      </rq:DocTypeCode>
    </xsl:if>
    <xsl:if test="rqi:DocTypeName != ''">
      <rq:DocTypeName>
        <xsl:value-of select="rqi:DocTypeName"/>
      </rq:DocTypeName>
    </xsl:if>
    <xsl:if test="rqi:WorkType != ''">
      <rq:WorkType>
        <xsl:value-of select="rqi:WorkType"/>
      </rq:WorkType>
    </xsl:if>
    <xsl:if test="rqi:Signature != ''">
      <rq:Signature>
        <xsl:value-of select="rqi:Signature"/>
      </rq:Signature>
    </xsl:if>
    <xsl:if test="rqi:Subjects != ''">
      <rq:Subjects>
        <xsl:value-of select="rqi:Subjects"/>
      </rq:Subjects>
    </xsl:if>
    <xsl:if test="rqi:IndexTerms != ''">
      <rq:IndexTerms>
        <xsl:value-of select="rqi:IndexTerms"/>
      </rq:IndexTerms>
    </xsl:if>
    <xsl:if test="rqi:AboutPersons != ''">
      <rq:AboutPersons>
        <xsl:value-of select="rqi:AboutPersons"/>
      </rq:AboutPersons>
    </xsl:if>
    <xsl:if test="rqi:AboutLocation != ''">
      <rq:AboutLocation>
        <xsl:value-of select="rqi:AboutLocation"/>
      </rq:AboutLocation>
    </xsl:if>
    <xsl:if test="rqi:AboutTime != ''">
      <rq:AboutTime>
        <xsl:value-of select="rqi:AboutTime"/>
      </rq:AboutTime>
    </xsl:if>
		<xsl:if test="rqi:ClassificationFieldContent != ''">
      <rq:Classification>
        <xsl:value-of select="rqi:ClassificationFieldContent"/>
      </rq:Classification>
    </xsl:if>
    <xsl:if test="rqi:Abstract != ''">
      <rq:Abstract>
        <xsl:value-of select="rqi:Abstract"/>
      </rq:Abstract>
    </xsl:if>
    <xsl:if test="rqi:CreateLocation != ''">
      <rq:CreateLocation>
        <xsl:value-of select="rqi:CreateLocation"/>
      </rq:CreateLocation>
    </xsl:if>
    <xsl:if test="rqi:CreateTime != ''">
      <rq:CreateTime>
        <xsl:value-of select="rqi:CreateTime"/>
      </rq:CreateTime>
    </xsl:if>
    <xsl:if test="rqi:Notes != ''">
      <rq:Notes>
        <xsl:value-of select="rqi:Notes"/>
      </rq:Notes>
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
	