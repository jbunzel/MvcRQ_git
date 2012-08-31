<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
	xmlns:rqicl="http://schemas.datacontract.org/2004/07/RQLib.RQKos.Classifications"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:mo="http://www.loc.gov/mods/v3">

	<xsl:output omit-xml-declaration="yes"/>

	<xsl:template match="@* | node()">
		<xsl:apply-templates select="@* | node()"/>
	</xsl:template>

	<xsl:template match="rqicl:SubjClass">
		<xsl:element name="a">
			<xsl:attribute name="href">
				<xsl:value-of select="rqicl:ClassID"/>
			</xsl:attribute>
			<xsl:attribute name="title">
				<xsl:value-of select="'Alle Dokumente in dieser Klasse'"/>
			</xsl:attribute>
			<xsl:value-of select="rqicl:ClassCode"/>
		</xsl:element>
		<xsl:value-of select="' - '"/>
		<xsl:choose>
			<xsl:when test="string-length(rqicl:ClassShortTitle) > 0">
				<xsl:element name="a">
					<xsl:attribute name="href">
						<xsl:value-of select="rqicl:ClassID"/>
					</xsl:attribute>
					<xsl:attribute name="title">
						<xsl:value-of select="'Linked Data Informationen zu dieser Klasse'"/>
					</xsl:attribute>
					<xsl:value-of select="rqicl:ClassShortTitle"/>
				</xsl:element>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="'(not available)'"/>
			</xsl:otherwise>
		</xsl:choose>
		<xsl:value-of select="'; '"/>
	</xsl:template>

</xsl:stylesheet>
