<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" version="1.0">

  <xsl:output encoding="utf-8" indent="yes" omit-xml-declaration="no"/>
  
	<xsl:template match="/">
	    <xsl:element name="rdf:RDF">
	    	<xsl:for-each select="//RQKosItemTemplate[1]//rdf:RDF/namespace::*">
	    		<xsl:copy />
	    	</xsl:for-each>
	    	<xsl:apply-templates select="//RDF" />
	    </xsl:element>	
	</xsl:template>
	
	<xsl:template match="RDF">
		<xsl:copy-of select="rdf:RDF/*"/>
	</xsl:template> 

</xsl:stylesheet>

