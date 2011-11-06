<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:rq="http://riquest.de/formats/rq"
	xmlns:dc="http://purl.org/dc/elements/1.1/"
	xmlns:srw_dc="info:srw/schema/1/dc-schema"
	xmlns:oai_dc="http://www.openarchives.org/OAI/2.0/oai_dc/"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt">

  <!-- 
This stylesheet maps RQ version 0.1 records to itself 

Revision 0.0	yyyy-mm-dd <jbunzel@riquest.de>
	
Version 0.1	2010-04-07 Jürgen Bunzel <jbunzel@riquest.de>

-->

  <xsl:output method="xml" indent="yes"/>


  <xsl:template match="/">
    <xsl:copy-of select="/" />
  </xsl:template>

  <!-- template provided with 2 typos by Jeni Tennison, 25 Feb 2001 -->
  <xsl:template name="escape-apos">
    <xsl:param name="string" />

    <xsl:value-of select="$string" disable-output-escaping="no"/>
  </xsl:template>

</xsl:stylesheet>
