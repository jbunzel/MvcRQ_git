<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:rq="http://riquest.de/formats/rq"
	xmlns:dc="http://purl.org/dc/elements/1.1/"
	xmlns:srw_dc="info:srw/schema/1/dc-schema"
	xmlns:oai_dc="http://www.openarchives.org/OAI/2.0/oai_dc/"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="rq">
<!-- 
This stylesheet transforms RQ version 0.1 records and collections of records to simple Dublin Core (DC) records 
		
The stylesheet will transform a collection of RQ 0.1 records into simple Dublin Core (DC)
as expressed by the SRU DC schema <http://www.loc.gov/standards/sru/dc-schema.xsd>

The stylesheet will transform a single RQ 0.1 record into simple Dublin Core (DC)
as expressed by the OAI DC schema <http://www.openarchives.org/OAI/2.0/oai_dc.xsd>
		
This stylesheet makes the following decisions in its interpretation of the RQ to simple DC mapping: 
	
When the roleTerm value associated with a name is creator, then name maps to dc:creator
When there is no roleTerm value associated with name, or the roleTerm value associated with name is a value other than creator, then name maps to dc:contributor
Start and end dates are presented as span dates in dc:date and in dc:coverage
When the first subelement in a subject wrapper is topic, subject subelements are strung together in dc:subject with hyphens separating them
Some subject subelements, i.e., geographic, temporal, hierarchicalGeographic, and cartographics, are also parsed into dc:coverage
The subject subelement geographicCode is dropped in the transform


Revision 0.0	yyyy-mm-dd <jbunzel@riquest.de>
	
Version 0.1	2010-03-20 Jürgen Bunzel <jbunzel@riquest.de>

-->

  <xsl:output method="xml" indent="yes"/>

  <xsl:param name="interface" select="'UNAPI'" />

  <xsl:template name="ParseSubFields">
    <xsl:param name="FieldSubString" />

    <xsl:variable name="Head" select="normalize-space(substring-before($FieldSubString,';'))" />
    <xsl:if test="$Head">
      <xsl:element name="SubFields">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "$Head" />
        </xsl:call-template>
      </xsl:element>
    </xsl:if>
    <xsl:variable name="Tail" select="normalize-space(substring-after($FieldSubString,';'))" />
    <xsl:if test="$Tail">
      <xsl:call-template name="ParseSubFields">
        <xsl:with-param name="FieldSubString" select="$Tail" />
      </xsl:call-template>
    </xsl:if>
  </xsl:template>


  <xsl:template match="/">
    <xsl:choose>
      <xsl:when test="//ArrayOfRQItem">
        <xsl:choose>
          <xsl:when test="$interface='SRU'">
            <xsl:element name="records">
              <xsl:apply-templates />
              <xsl:for-each select="ArrayOfRQItem/RQItem">
                <xsl:element name="record">
                  <xsl:element name="recordSchema">
                    <xsl:value-of select="'info:srw/schema/1/dc-schema http://www.loc.gov/standards/sru/dc-schema.xsd'" />
                  </xsl:element>
                  <xsl:element name="recordPacking">
                    <xsl:value-of select="'xml'" />
                  </xsl:element>
                  <xsl:element name="recordData">
                    <srw_dc:dc xsi:schemaLocation="info:srw/schema/1/dc-schema http://www.loc.gov/standards/sru/dc-schema.xsd">
                      <xsl:call-template name="DCRecord" />
                    </srw_dc:dc>
                  </xsl:element>
                </xsl:element>
              </xsl:for-each>
            </xsl:element>
          </xsl:when>
          <xsl:otherwise>
            <srw_dc:dcCollection xsi:schemaLocation="info:srw/schema/1/dc-schema http://www.loc.gov/standards/sru/dc-schema.xsd">
              <xsl:apply-templates />
              <xsl:for-each select="ArrayOfRQItem/RQItem">
                <srw_dc:dc xsi:schemaLocation="info:srw/schema/1/dc-schema http://www.loc.gov/standards/sru/dc-schema.xsd">
                  <xsl:call-template name="DCRecord" />
                </srw_dc:dc>
              </xsl:for-each>
            </srw_dc:dcCollection>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:otherwise>
        <xsl:for-each select="RQItem">
          <oai_dc:dc xsi:schemaLocation="http://www.openarchives.org/OAI/2.0/oai_dc/ http://www.openarchives.org/OAI/2.0/oai_dc.xsd">
            <xsl:call-template name="DCRecord" />
          </oai_dc:dc>
        </xsl:for-each>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <xsl:template name="DCRecord">
    <xsl:if test="Title">
      <dc:title>
        <xsl:value-of select="Title" />
      </dc:title>
    </xsl:if>
    <xsl:if test="Authors">
      <xsl:variable name="Subfields">
        <xsl:call-template name="ParseSubFields">
          <xsl:with-param name="FieldSubString" select="concat(Authors,';')" />
        </xsl:call-template>
      </xsl:variable>

      <xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
        <xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
          <dc:creator>
            <xsl:value-of select="." />
          </dc:creator>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
    <xsl:if test="Subjects | IndexTerms | AboutPersons">
      <!-- Classification not covered -->
      <xsl:variable name="Subfields">
        <xsl:call-template name="ParseSubFields">
          <xsl:with-param name="FieldSubString" select="concat(concat(Subjects,';'),concat(IndexTerms,';'),concat(AboutPersons,';'))" />
        </xsl:call-template>
      </xsl:variable>

      <xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
        <xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
          <dc:subject>
            <xsl:value-of select="." />
          </dc:subject>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
    <xsl:if test="Abstract">
      <xsl:if test="not(contains(Abstract,'$$'))">
        <dc:description>
          <xsl:value-of select="Abstract" />
        </dc:description>
      </xsl:if>
    </xsl:if>
    <xsl:if test="Publisher">
      <xsl:variable name="Subfields">
        <xsl:call-template name="ParseSubFields">
          <xsl:with-param name="FieldSubString" select="concat(Publisher,';')" />
        </xsl:call-template>
      </xsl:variable>

      <xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
        <xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
          <dc:publisher>
            <xsl:value-of select="." />
          </dc:publisher>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
    <xsl:if test="Authors | Institutions">
      <xsl:variable name="Subfields">
        <xsl:call-template name="ParseSubFields">
          <xsl:with-param name="FieldSubString" select="concat(concat(Institutions,';'),concat(Authors,';'))" />
        </xsl:call-template>
      </xsl:variable>

      <xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
        <xsl:if test="contains(.,'{') and not(contains(.,'=>'))">
          <dc:contributor>
            <xsl:value-of select="." />
          </dc:contributor>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
    <xsl:if test="PublTime">
      <!-- CreateTime not covered -->
      <xsl:variable name="Subfields">
        <xsl:call-template name="ParseSubFields">
          <xsl:with-param name="FieldSubString" select="concat(PublTime,';')" />
        </xsl:call-template>
      </xsl:variable>

      <xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
        <xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
          <dc:date>
            <xsl:value-of select="." />
          </dc:date>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
    <xsl:if test="DocTypeName | WorkType">
      <xsl:variable name="Subfields">
        <xsl:call-template name="ParseSubFields">
          <xsl:with-param name="FieldSubString" select="concat(concat(DocTypeName,';'),concat(WorkType,';'))" />
        </xsl:call-template>
      </xsl:variable>

      <xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
        <xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
          <dc:type>
            <xsl:value-of select="." />
          </dc:type>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
    <xsl:if test="Pages">
      <dc:format>
        <xsl:value-of select="Pages" />
      </dc:format>
    </xsl:if>
    <xsl:if test="ISDN | Coden">
      <xsl:variable name="Subfields">
        <xsl:call-template name="ParseSubFields">
          <xsl:with-param name="FieldSubString" select="concat(concat(ISDN,';'),concat(Coden,';'))" />
        </xsl:call-template>
      </xsl:variable>

      <xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
        <xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
          <dc:identifier>
            <xsl:value-of select="." />
          </dc:identifier>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
    <xsl:if test="Source">
      <xsl:variable name="Subfields">
        <xsl:call-template name="ParseSubFields">
          <xsl:with-param name="FieldSubString" select="concat(Source,';')" />
        </xsl:call-template>
      </xsl:variable>

      <xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
        <xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
          <dc:source>
            <xsl:value-of select="." />
          </dc:source>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
    <xsl:if test="Language">
      <xsl:variable name="Subfields">
        <xsl:call-template name="ParseSubFields">
          <xsl:with-param name="FieldSubString" select="concat(Language,';')" />
        </xsl:call-template>
      </xsl:variable>

      <xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
        <xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
          <dc:language>
            <xsl:value-of select="." />
          </dc:language>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
    <xsl:if test="Series">
      <xsl:variable name="Subfields">
        <xsl:call-template name="ParseSubFields">
          <xsl:with-param name="FieldSubString" select="concat(Series,';')" />
        </xsl:call-template>
      </xsl:variable>

      <xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
        <xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
          <dc:relation>
            <xsl:value-of select="." />
          </dc:relation>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
    <xsl:if test="AboutLocation | AboutTime">
      <xsl:variable name="Subfields">
        <xsl:call-template name="ParseSubFields">
          <xsl:with-param name="FieldSubString" select="concat(concat(AboutLocation,';'),concat(AboutTime,';'))" />
        </xsl:call-template>
      </xsl:variable>

      <xsl:for-each select="msxsl:node-set($Subfields)/SubFields">
        <xsl:if test="not(contains(.,'{')) and not(contains(.,'=>'))">
          <dc:coverage>
            <xsl:value-of select="." />
          </dc:coverage>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
    <xsl:if test="RQRights">
      <dc:rights>
        <xsl:value-of select="RQRights" />
      </dc:rights>
    </xsl:if>
  </xsl:template>

  <!-- suppress all else:-->
  <xsl:template match="*">
  </xsl:template>

  <!-- template provided with 2 typos by Jeni Tennison, 25 Feb 2001 -->
  <xsl:template name="escape-apos">
    <xsl:param name="string" />

    <xsl:value-of select="$string" disable-output-escaping="no"/>
  </xsl:template>

</xsl:stylesheet>
