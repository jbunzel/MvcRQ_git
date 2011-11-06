<xsl:stylesheet	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                xmlns:mo="http://www.loc.gov/mods/v3">

  <xsl:output omit-xml-declaration="yes"/>
  
  <xsl:param name="MyDocsPath" select="''" />
  <xsl:param name="MyMusicPath" select="''" />


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


  <xsl:template name="generate-toc">
    <xsl:param name="tocstr" select="''" />

    <tr>
      <td class="fieldvalue">
        <xsl:variable name="Signatures">
          <xsl:call-template name="ParseSubFields">
            <xsl:with-param name="FieldSubString" select="concat($tocstr,'; ')" />
          </xsl:call-template>
        </xsl:variable>

        <table>
          <xsl:for-each select="msxsl:node-set($Signatures)/SubFields">
            <tr>
              <td class="fieldvalue" valign="top">
                <xsl:choose>
                  <xsl:when test="contains(.,'MyDoc=')">
                    <xsl:call-template name = "escape-apos">
                      <xsl:with-param name = "string" select = "concat(substring-before(.,'MyDoc='),'MyDoc=')" />
                    </xsl:call-template>
                    <a>
                      <xsl:attribute name="class">
                        <xsl:text>folderitem</xsl:text>
                      </xsl:attribute>
                      <xsl:attribute name="href">
                        <xsl:call-template name = "escape-apos">
                          <xsl:with-param name = "string" select = "concat($MyDocsPath,'/',substring-after(.,'MyDoc='))" />
                        </xsl:call-template>
                      </xsl:attribute>
                      <xsl:attribute name="target">
                        <xsl:text>_new</xsl:text>
                      </xsl:attribute>
                      <xsl:call-template name = "escape-apos">
                        <xsl:with-param name = "string" select = "substring-after(.,'MyDoc=')" />
                      </xsl:call-template>
                    </a>
                  </xsl:when>
                  <xsl:when test="contains(.,'MyMusic=')">
                    <a>
                      <xsl:attribute name="class">
                        <xsl:text>folderitem</xsl:text>
                      </xsl:attribute>
                      <xsl:attribute name="href">
                        <xsl:call-template name = "escape-apos">
                          <xsl:with-param name = "string" select = "concat($MyMusicPath,'/',substring-after(.,'MyMusic='))" />
                        </xsl:call-template>
                      </xsl:attribute>
                      <xsl:attribute name="target">
                        <xsl:text>_new</xsl:text>
                      </xsl:attribute>
                      <xsl:call-template name = "escape-apos">
                        <xsl:with-param name = "string" select = "substring-before(.,'MyMusic=')" />
                      </xsl:call-template>
                    </a>
                  </xsl:when>
                  <xsl:when test="contains(.,'RQItem=')">
                    <a>
                      <xsl:attribute name="class">
                        <xsl:text>folderitem</xsl:text>
                      </xsl:attribute>
                      <xsl:attribute name="href">
                        <xsl:call-template name = "escape-apos">
                          <xsl:with-param name = "string" select = "substring-after(.,'RQItem=')" />
                        </xsl:call-template>
                      </xsl:attribute>
                      <xsl:attribute name="target">
                        <xsl:text>_new</xsl:text>
                      </xsl:attribute>
                      <xsl:call-template name = "escape-apos">
                        <xsl:with-param name = "string" select = "substring-before(.,'RQItem=')" />
                      </xsl:call-template>
                    </a>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:call-template name = "escape-apos">
                      <xsl:with-param name = "string" select = "." />
                    </xsl:call-template>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </td>
    </tr>
  </xsl:template>


  <xsl:template name="RenderSingleView">
    <table class="singleitem">
      <xsl:apply-templates select="*[name() !='ID']" mode="item"/>
    </table>
  </xsl:template>

  
  <xsl:template match="/">
		<table border="0" cellspacing="0" cellpadding="2">
			<xsl:apply-templates select="//RQItem"/>
		</table>
	</xsl:template>
	
  
	<xsl:template match="RQItem">
    <tr>
      <td>
        <xsl:call-template name="RenderSingleView"/>
        <p/>
      </td>
    </tr>
  </xsl:template>

  
  <xsl:template match="RQItem/*[(name() !='file') and (string(.) !='') and (string(.) != ' ')]"	mode="item">
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
  </xsl:template>


  <xsl:template match="RQItem/Abstract">
    <table>
      <tr>
        <td class="comment">
          <xsl:choose>
            <xsl:when test="substring-after(.,'$$TOC$$=')">
              <xsl:call-template name = "escape-apos">
                <xsl:with-param name = "string" select = "substring-before(.,'$$TOC$$=')" />
              </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
              <xsl:call-template name = "escape-apos">
                <xsl:with-param name = "string" select = "." />
              </xsl:call-template>
            </xsl:otherwise>
          </xsl:choose>
        </td>
      </tr>
      <xsl:if test="substring-after(.,'$$TOC$$=')">
        <xsl:call-template name="generate-toc">
          <xsl:with-param name="tocstr" select="substring-after(.,'$$TOC$$=')" />
        </xsl:call-template>
      </xsl:if>
    </table>
  </xsl:template>


  <!-- template provided with 2 typos by Jeni Tennison, 25 Feb 2001 -->
  <xsl:template name="escape-apos">
    <xsl:param name="string"/>

    <xsl:value-of select="$string" disable-output-escaping="no"/>
  </xsl:template>

</xsl:stylesheet>
