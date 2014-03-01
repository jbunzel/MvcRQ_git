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
		<xsl:param name="FieldSubString" select="."/>
		
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
    <xsl:param name="SubField" select="."/>
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

  
  <xsl:template name="TrimEnd">
    <xsl:param name="Input" select="." />
    <xsl:param name="TrimChar" select="';'" />

    <xsl:choose>
      <xsl:when test="substring($Input, string-length($Input) - string-length($TrimChar) + 1) = $TrimChar">
        <xsl:value-of select="substring($Input, 0, string-length($Input) - string-length($TrimChar) + 1)"/>
      </xsl:when>  
      <xsl:otherwise>
        <xsl:value-of select="$Input"/>
      </xsl:otherwise>    
    </xsl:choose>
  </xsl:template>

  
  <xsl:template name="RenderShortTitleView">
    <xsl:apply-templates select="rqi:Title" mode="item"/>
    <xsl:choose>
      <xsl:when test="contains(rqi:DocTypeName, 'Aufsatz')">
        <xsl:apply-templates select="rqi:Source" mode="initem"/>
        <xsl:apply-templates select="rqi:Pages" mode="item"/>
        <xsl:apply-templates select="rqi:ISDN" mode="initem"/>
      </xsl:when>
      <xsl:when test="contains(rqi:DocTypeName, 'Beitrag') or contains(rqi:DocTypeName, 'Artikel')">
        <xsl:apply-templates select="rqi:Source" mode="item"/>
        <xsl:apply-templates select="rqi:Edition" mode="item"/>
        <xsl:apply-templates select="rqi:Locality" mode="item"/>
        <xsl:apply-templates select="rqi:Publisher" mode="item"/>
        <xsl:apply-templates select="rqi:PublTime" mode="item"/>
        <xsl:apply-templates select="rqi:Pages" mode="item"/>
        <xsl:apply-templates select="rqi:Series" mode="item"/>
        <xsl:apply-templates select="rqi:ISDN" mode="item"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates select="rqi:Edition" mode="item"/>
        <xsl:apply-templates select="rqi:Locality" mode="item"/>
        <xsl:apply-templates select="rqi:Publisher" mode="item"/>
        <xsl:apply-templates select="rqi:PublTime" mode="item"/>
        <xsl:apply-templates select="rqi:Pages" mode="item"/>
        <xsl:apply-templates select="rqi:Series" mode="item"/>
        <xsl:apply-templates select="rqi:ISDN" mode="item"/>
      </xsl:otherwise>    
    </xsl:choose>
  </xsl:template>
	
  
	<xsl:template match="/">
    <div class="bibinfo">
			<xsl:apply-templates select="//rqi:RQItem"/>
		</div>
  </xsl:template>
	
  
	<xsl:template match="rqi:RQItem">
  	<xsl:call-template name="RenderShortTitleView"/>
	</xsl:template>

  
	<xsl:template match="rqi:RQItem/*[(name() !='file') and (string(.) !='') and (string(.) != ' ')]" mode="item">
    <xsl:variable name="out">
      <xsl:call-template name="TrimEnd" />
    </xsl:variable>
  
    <xsl:value-of select="concat('. - ', $out)" />
	</xsl:template>

  
  <xsl:template match="rqi:RQItem/rqi:Title" mode="item">
    <xsl:value-of select="."/>
    <xsl:if test ="not(contains(., '/'))">
      <xsl:value-of select ="' / '" />
      <xsl:apply-templates select="../rqi:Authors" mode="item"/>
    </xsl:if>
  </xsl:template>

  
  <xsl:template match="rqi:RQItem/rqi:Authors" mode="item">
    <xsl:choose>
      <xsl:when test="../rqi:AuthorsEntity/a:items/a:RQDescriptionComponent">
        <xsl:for-each select="../rqi:AuthorsEntity/a:items/a:RQDescriptionComponent">
          <xsl:if test="position() &lt; 4">
            <xsl:if test="not(contains(bb:PersonalName, '(='))" >
              <xsl:variable name="sname">
                <xsl:choose>
                  <xsl:when test="contains(bb:PersonalName, ', ')">
                    <xsl:value-of select="normalize-space(substring-before(bb:PersonalName, ', '))"/>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="''" />
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:variable>

              <xsl:variable name="tail">
                <xsl:choose>
                  <xsl:when test="contains(bb:PersonalName, ', ')">
                    <xsl:value-of select="normalize-space(substring-after(bb:PersonalName, ', '))"/>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="normalize-space(bb:PersonalName)" />
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:variable>

              <xsl:variable name="func">
                <xsl:if test="contains($tail, '{')">
                  <xsl:value-of select="concat('{', normalize-space(substring-after($tail, '{')))"/>
                </xsl:if>
              </xsl:variable>

              <xsl:variable name="pname">
                <xsl:choose>
                  <xsl:when test="contains($tail, '&lt;')" >
                    <xsl:value-of select="normalize-space(substring-before($tail, '&lt;'))"/>
                  </xsl:when>
                  <xsl:when test="contains($tail, '(')" >
                    <xsl:value-of select="normalize-space(substring-before($tail, '('))"/>
                  </xsl:when>
                  <xsl:when test="contains($tail, '{')" >
                    <xsl:value-of select="normalize-space(substring-before($tail, '{'))"/>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="$tail"/>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:variable>

              <xsl:if test="position() &gt; 1">
                <xsl:value-of select="'; '"/>
              </xsl:if>
              <xsl:value-of select="$pname" />
              <xsl:if test="string-length($sname) > 0">
                <xsl:value-of select="concat(' ', $sname)"/>
              </xsl:if>
              <xsl:if test="string-length($func) > 0">
                <xsl:value-of select="concat(' ', $func)"/>
              </xsl:if>
            </xsl:if>
          </xsl:if>  
        </xsl:for-each>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates select="../rqi:Institutions" mode="item"/>
      </xsl:otherwise>
    </xsl:choose>        
  </xsl:template>
  
  
  <xsl:template match="rqi:RQItem/rqi:Institutions" mode="item">
    <xsl:variable name="Institutions">
      <xsl:call-template name="ParseSubFields">
        <xsl:with-param name="FieldSubString" select="concat(., '; ')" />
      </xsl:call-template>
    </xsl:variable>

    <xsl:if test="msxsl:node-set($Institutions)/SubFields">
      <xsl:for-each select="msxsl:node-set($Institutions)/SubFields">
        <xsl:if test="position() &lt; 4">
          <xsl:if test="position() &gt; 1">
            <xsl:value-of select="'; '"/>
          </xsl:if>
          <xsl:value-of select="."/>    
        </xsl:if>
      </xsl:for-each>  
    </xsl:if>
  </xsl:template>

  
  <xsl:template match="rqi:RQItem/rqi:Locality" mode="item">
    <xsl:choose>
      <xsl:when test="string-length(.) > 0">
        <xsl:call-template name="OutputSubFields" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="'. - o. O.'"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  
  <xsl:template match="rqi:RQItem/rqi:Publisher" mode="item">
    <xsl:if test="string-length(.) > 0">
      <xsl:call-template name="OutputSubFields">
        <xsl:with-param name="Praefix" select="' : '" />
      </xsl:call-template>
    </xsl:if>
  </xsl:template>

  
  <xsl:template match="rqi:RQItem/rqi:PublTime" mode="item">
    <xsl:if test="string-length(.) > 0">
      <xsl:call-template name="OutputSubFields">
        <xsl:with-param name="Praefix" select="', '" />
      </xsl:call-template>
    </xsl:if>
  </xsl:template>

  
  <xsl:template match="rqi:RQItem/rqi:Series" mode="item">
    <xsl:if test="string-length(.) > 0">
      <br/>
      <xsl:variable name="out">
        <xsl:call-template name="TrimEnd" />
      </xsl:variable>
      
      <xsl:value-of select="concat('(',$out)"/>
      <xsl:if test="string-length(../rqi:Volume) > 0">
        <xsl:value-of select="concat(' ; ',../rqi:Volume)"/>
      </xsl:if>
      <xsl:value-of select="')'"/>
    </xsl:if>
  </xsl:template>

  
  <xsl:template match="rqi:RQItem/rqi:Source" mode="item">
    <xsl:if test="string-length(../rqi:Source) > 0">
      <xsl:variable name="out">
        <xsl:call-template name="TrimEnd">
          <xsl:with-param name="Input" select="../rqi:Source" />
        </xsl:call-template>
      </xsl:variable>

      <xsl:choose>
        <xsl:when test="starts-with($out, 'in')">
          <xsl:value-of select="concat('. - ',$out)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="concat('. - in: ',$out)"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
  </xsl:template>
  
  
  <xsl:template match="rqi:RQItem/rqi:Source" mode="initem">
    <xsl:if test="string-length(.) > 0">
      <xsl:variable name="out">
        <xsl:call-template name="TrimEnd">
          <xsl:with-param name="Input" select="." />
        </xsl:call-template>
      </xsl:variable>

      <xsl:value-of select="concat('. - ', $out)"/>
    </xsl:if>
    <xsl:if test="string-length(../rqi:Volume) > 0">
      <xsl:variable name="out">
        <xsl:call-template name="TrimEnd">
          <xsl:with-param name="Input" select="../rqi:Volume" />
        </xsl:call-template>
      </xsl:variable>

      <xsl:value-of select="concat('. - ',$out)"/>
    </xsl:if>
    <xsl:if test="string-length(../rqi:PublTime) > 0">
      <xsl:variable name="out">
        <xsl:call-template name="TrimEnd">
          <xsl:with-param name="Input" select="../rqi:PublTime" />
        </xsl:call-template>
      </xsl:variable>

      <xsl:value-of select="concat('(',$out, ')')"/>
    </xsl:if>
    <xsl:if test="string-length(../rqi:Issue) > 0">
      <xsl:value-of select="../rqi:Issue"/>
    </xsl:if>
  </xsl:template>
  
  
  <xsl:template match="rqi:RQItem/rqi:ISDN" mode="initem">
    <xsl:if test="string-length(.) > 0" >
      <xsl:variable name="list">
        <xsl:call-template name="ParseSubFields" />
      </xsl:variable>
      
      <br/>
      <xsl:choose>
        <xsl:when test="not(starts-with(., 'ISSN'))">
          <xsl:value-of select="concat('ISSN: ', msxsl:node-set($list)/SubFields[0])"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="msxsl:node-set($list)/SubFields[0]"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
  </xsl:template>
  
  
  <xsl:template match="rqi:RQItem/rqi:ISDN" mode="item">
    <xsl:if test="string-length(.) > 0" >
      <xsl:variable name="list">
        <xsl:call-template name="ParseSubFields" />
      </xsl:variable>
  
      <br/>
      <xsl:choose>
        <xsl:when test="not(starts-with(., 'ISBN'))">
          <xsl:value-of select="concat('ISBN: ', msxsl:node-set($list)/SubFields[1])"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="msxsl:node-set($list)/SubFields[1]"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
  </xsl:template>

  
  <!-- template provided with 2 typos by Jeni Tennison, 25 Feb 2001 -->
	<xsl:template name="escape-apos">
		<xsl:param name="string"/>
		
		<xsl:value-of select="$string" disable-output-escaping="no"/>
	</xsl:template>
	
</xsl:stylesheet>