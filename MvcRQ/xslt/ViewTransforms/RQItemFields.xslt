<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
	xmlns:rqicl="http://schemas.datacontract.org/2004/07/RQLib.RQKos.Classifications"
	xmlns:rqipe="http://schemas.datacontract.org/2004/07/RQLib.RQKos.Persons"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:mo="http://www.loc.gov/mods/v3">

	<xsl:output omit-xml-declaration="yes"/>

	<xsl:template match="@* | node()">
		<xsl:apply-templates select="@* | node()"/>
	</xsl:template>

	<xsl:template match="rqicl:SubjClass">
    <xsl:choose>
      <xsl:when test="string-length(rqicl:ClassShortTitle) > 0">
          <xsl:value-of select="rqicl:ClassShortTitle"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="'not available'"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="rqipe:Person">
    <xsl:value-of select="'not available'"/>
  </xsl:template>

</xsl:stylesheet>
