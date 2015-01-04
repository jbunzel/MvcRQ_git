<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template match="/">

		<SCRIPT LANGUAGE="JavaScript">
			function visibilityToggle(id) {
				var el = document.getElementById(id).style;

				if(el.display == "none") {
					el.display = "block";
				}
				else if(el.display == "block") {
					el.display = "none";
				}
			}
		</SCRIPT>

		<xsl:apply-templates select="FAQList/FAQItem"/>
	</xsl:template>

	<xsl:template match="FAQItem">
		<xsl:element name="div">
			<xsl:attribute name="class">
				<xsl:text>fieldname</xsl:text>
			</xsl:attribute>
			<xsl:value-of select="concat(format-number(position(),'00'),'. ')" />
			<xsl:apply-templates select="FAQQuestion" />
		</xsl:element>
	</xsl:template>

	
	<xsl:template match="FAQQuestion">
		<xsl:variable name="EID">
			<xsl:value-of select="generate-id()"/>
		</xsl:variable>

		<xsl:if test="string-length(.) > 0">
			<xsl:element name="a">
				<xsl:attribute name="href">
					<xsl:value-of select="'#'" />
				</xsl:attribute>
				<xsl:attribute name="onclick">
					<xsl:text>visibilityToggle('</xsl:text>
					<xsl:value-of select="$EID" />
					<xsl:text>')</xsl:text>
				</xsl:attribute>
<!--        
        <xsl:attribute name="onmouseout">
          <xsl:text>visibilityToggle('</xsl:text>
          <xsl:value-of select="$EID" />
          <xsl:text>')</xsl:text>
        </xsl:attribute>
-->        
        <xsl:attribute name="class">
					<xsl:text>folderitem</xsl:text>
				</xsl:attribute>
				<xsl:value-of select="." />
			</xsl:element>
		</xsl:if>
		<xsl:if test="string-length(../FAQQuestion) > 0">
			<xsl:element name="div">
				<xsl:attribute name="ID">
					<xsl:value-of select="$EID" />
				</xsl:attribute>
				<xsl:attribute name="style">
					<xsl:text>display : none;</xsl:text>
				</xsl:attribute>
				<table border="0" cellspacing="0" cellpadding="10">
					<tr>
						<td class="itemlist comment">
							<xsl:call-template name = "escape-apos">
								<xsl:with-param name = "string" select = "../FAQAnswer" />
							</xsl:call-template>
						</td>
					</tr>
				</table>
			</xsl:element>
		</xsl:if>
	</xsl:template>

	<!-- template provided with 2 typos by Jeni Tennison, 25 Feb 2001 -->
	<xsl:template name="escape-apos">
		<xsl:param name="string" />

		<xsl:value-of select="$string" disable-output-escaping="yes"/>
	</xsl:template>

</xsl:stylesheet>

