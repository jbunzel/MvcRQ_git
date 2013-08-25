<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0" 	
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
	xmlns:rqi="http://schemas.datacontract.org/2004/07/MvcRQ.Models"
	xmlns:b="http://schemas.datacontract.org/2004/07/RQLib.RQKos.Classifications" 
  xmlns:bb="http://schemas.datacontract.org/2004/07/RQLib.RQKos.Persons"              
	xmlns:a="http://schemas.datacontract.org/2004/07/RQLib.RQQueryResult.RQDescriptionElements">

	<xsl:output omit-xml-declaration="yes"/>
	
	<xsl:param name="MyDocsPath" select="''"/>
	<xsl:param name="MyMusicPath" select="''"/>
		
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

  <xsl:template name="OutputSubFields">
    <xsl:param name="SubField"/>
    <xsl:param name="Praefix" select="'. - '" />
    <xsl:param name="Separator" select="'; '" />
    
    <xsl:variable name="FieldType">
      <xsl:call-template name="ParseSubFields">
        <xsl:with-param name="FieldSubString" select="concat($SubField,'; ')"/>
      </xsl:call-template>
    </xsl:variable>

    <xsl:value-of select="$Praefix"/>
    <xsl:for-each select="msxsl:node-set($FieldType)/SubFields">
      <xsl:if test="position() > 1" >
        <xsl:value-of select="$Separator"/>
      </xsl:if>
      <xsl:value-of select="." />
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="RenderShortTitleView">
      <xsl:apply-templates select="rqi:Title" mode="item"/>
      <xsl:if test ="not(contains(rqi:Title, '/'))">
        <xsl:value-of select ="' / '" />
        <xsl:apply-templates select="rqi:AuthorsEntity" mode="item"/>
        <xsl:value-of select ="'. '" />
        <xsl:apply-templates select="rqi:Institutions" mode="item"/>
      </xsl:if>
      <xsl:apply-templates select="rqi:Source" mode="item"/>
      <xsl:apply-templates select="rqi:Volume" mode="item"/>
      <xsl:apply-templates select="rqi:Issue" mode="item"/>
      <xsl:apply-templates select="rqi:Edition" mode="item"/>
      <xsl:apply-templates select="rqi:Locality" mode="item"/>
      <xsl:apply-templates select="rqi:Publisher" mode="item"/>
      <xsl:apply-templates select="rqi:PublTime" mode="item"/>
      <xsl:apply-templates select="rqi:Pages" mode="item"/>
      <xsl:apply-templates select="rqi:ISDN" mode="item"/>
<!--    
      <xsl:apply-templates select="rqi:Coden" mode="item"/>
      <xsl:apply-templates select="rqi:Language" mode="item"/>
      <xsl:apply-templates select="rqi:Series" mode="item"/>
      <xsl:apply-templates select="rqi:Signature" mode="item"/>
      <xsl:apply-templates select="rqi:DocTypeCode" mode="item"/>
      <xsl:apply-templates select="rqi:DocTypeName" mode="item"/>
      <xsl:apply-templates select="rqi:WorkType" mode="item"/>
      <xsl:apply-templates select="rqi:Subjects" mode="item"/>
      <xsl:apply-templates select="rqi:Classification" mode="item"/>
      <xsl:apply-templates select="rqi:IndexTerms" mode="item"/>
      <xsl:apply-templates select="rqi:AboutPersons" mode="item"/>
      <xsl:apply-templates select="rqi:AboutLocation" mode="item"/>
      <xsl:apply-templates select="rqi:AboutTime" mode="item"/>
      <xsl:apply-templates select="rqi:Abstract" mode="item"/>
      <xsl:apply-templates select="rqi:CreateLocation" mode="item"/>
      <xsl:apply-templates select="rqi:CreateTime" mode="item"/>
      <xsl:apply-templates select="rqi:Notes" mode="item"/>
-->
  </xsl:template>
		
	<xsl:template match="/">
    <div class="bibinfo">
			<xsl:apply-templates select="//rqi:RQItem"/>
		</div>
  </xsl:template>
		
	<xsl:template match="rqi:RQItem">
    <xsl:if test="rqi:AccessRights">
<!--      
      <tr>
        <td>
          <xsl:variable name="rights" select="substring-after(rqi:AccessRights, 'actual=')" />
          <xsl:if test="contains($rights, 'edit')">
            <xsl:element name="a">
              <xsl:attribute name="href">
                <xsl:value-of select="concat('javascript:call(&quot;rqitems/',rqi:DocNo,'?verb=edit','&quot;)')" />
              </xsl:attribute>
              <xsl:text>edit</xsl:text>
            </xsl:element>
          </xsl:if>
          <xsl:if test="contains($rights, 'copy')">
            <xsl:text> - </xsl:text>
            <xsl:element name="a">
              <xsl:attribute name="href">
                <xsl:value-of select="concat('javascript:call(&quot;rqitems/',rqi:DocNo,'?verb=copy','&quot;)')" />
              </xsl:attribute>
              <xsl:text>copy</xsl:text>
            </xsl:element>
          </xsl:if>
          <xsl:if test="contains($rights, 'delete')">
            <xsl:text> - </xsl:text>
            <xsl:element name="a">
              <xsl:attribute name="href">
                <xsl:value-of select="concat('javascript:call(&quot;rqitems/',rqi:DocNo,'?verb=delete','&quot;)')" />
              </xsl:attribute>
              <xsl:text>delete</xsl:text>
            </xsl:element>
          </xsl:if>
        </td>
      </tr>
-->      
    </xsl:if>
  	<xsl:call-template name="RenderShortTitleView"/>
	</xsl:template>
		
	<xsl:template match="rqi:RQItem/*[(name() !='file') and (string(.) !='') and (string(.) != ' ')]" mode="item">
<!--
    <tr>
			<td class="fieldname" valign="top">
				<xsl:call-template name="escape-apos">
					<xsl:with-param name="string" select="name()"/>
				</xsl:call-template>
			</td>
			<td class="fieldvalue">
				<xsl:apply-templates select="."/>
			</td>
		</tr>
-->
    <xsl:value-of select="concat('. - ',.)"/>
	</xsl:template>

  <xsl:template match="rqi:RQItem/rqi:Title" mode="item">
    <xsl:value-of select="."/>
    <xsl:if test ="not(contains(., '/'))">
      
    </xsl:if>
  </xsl:template>

  <xsl:template match="rqi:RQItem/rqi:Locality" mode="item">
    <xsl:call-template name="OutputSubFields">
      <xsl:with-param name="SubField" select="."/>
    </xsl:call-template>
 <!--   
    <xsl:variable name="Locality">
      <xsl:call-template name="ParseSubFields">
        <xsl:with-param name="FieldSubString" select="concat(.,'; ')"/>
      </xsl:call-template>
    </xsl:variable>

    <xsl:value-of select="'. - '"/>
    <xsl:for-each select="msxsl:node-set($Locality)/SubFields">
      <xsl:if test="position() > 1" >
        <xsl:value-of select="'; '"/>
      </xsl:if>
      <xsl:value-of select="." />
    </xsl:for-each>
-->    
  </xsl:template>

  <xsl:template match="rqi:RQItem/rqi:Publisher" mode="item">
    <xsl:call-template name="OutputSubFields">
      <xsl:with-param name="SubField" select="."/>
      <xsl:with-param name="Praefix" select="' : '" />
    </xsl:call-template>
<!--    
    <xsl:value-of select="concat(' : ',.)"/>
-->    
  </xsl:template>

  <xsl:template match="rqi:RQItem/rqi:PublTime" mode="item">
    <xsl:value-of select="concat(', ',.)"/>
  </xsl:template>

  <!-- template provided with 2 typos by Jeni Tennison, 25 Feb 2001 -->
	<xsl:template name="escape-apos">
		<xsl:param name="string"/>
		
		<xsl:value-of select="$string" disable-output-escaping="no"/>
	</xsl:template>
	
</xsl:stylesheet>