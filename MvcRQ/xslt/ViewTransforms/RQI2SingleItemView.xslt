<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0" 	
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
	xmlns:rqi="http://schemas.datacontract.org/2004/07/MvcRQ.Models"
	xmlns:b="http://schemas.datacontract.org/2004/07/RQLib.RQKos.Classifications" 
  xmlns:bb="http://schemas.datacontract.org/2004/07/RQLib.RQKos.Persons"              
	xmlns:a="http://schemas.datacontract.org/2004/07/RQLib.RQQueryResult.RQDescriptionElements">

	<xsl:output omit-xml-declaration="yes"/>

  <xsl:param name ="ApplPath" select="''"/>
  <xsl:param name="MyDocsPath" select="''"/>
  <xsl:param name="MyVideoPath" select="''" />
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
	
	
	<xsl:template name="generate-toc">
		<xsl:param name="tocstr" select="''"/>
    <xsl:param name="DocNo" select="''" />
		
		<tr>
			<td class="fieldvalue">
				<xsl:variable name="Signatures">
					<xsl:call-template name="ParseSubFields">
						<xsl:with-param name="FieldSubString" select="concat($tocstr,'; ')"/>
					</xsl:call-template>
				</xsl:variable>
				
				<table>
					<xsl:for-each select="msxsl:node-set($Signatures)/SubFields">
						<tr>
							<td class="fieldvalue" valign="top">
								<xsl:choose>
									<xsl:when test="contains(.,'MyDoc=')">
<!--                    
										<xsl:call-template name="escape-apos">
											<xsl:with-param name="string"
												select="concat(substring-before(.,'MyDoc='),'MyDoc=')"
											/>
										</xsl:call-template>
-->                    
										<a>
											<xsl:attribute name="class">
												<xsl:text>folderitem</xsl:text>
											</xsl:attribute>
											<xsl:attribute name="href">
												<xsl:call-template name="escape-apos">
<!--                          
													<xsl:with-param name="string"
														select="concat($MyDocsPath,'/',substring-after(.,'MyDoc='))"
                          />
-->                          
                          <xsl:with-param name="string"
                            select = "concat($ApplPath, 'ItemViewer/', $DocNo, '?itemAdress=MyDocs/', substring-after(.,'MyDoc='))"
                          />
                        </xsl:call-template>
											</xsl:attribute>
											<xsl:attribute name="target">
												<xsl:text>_new</xsl:text>
											</xsl:attribute>
											<xsl:call-template name="escape-apos">
												<xsl:with-param name="string"
													select="substring-before(.,'MyDoc=')"/>
											</xsl:call-template>
										</a>
									</xsl:when>
									<xsl:when test="contains(.,'MyMusic=')">
										<a>
											<xsl:attribute name="class">
												<xsl:text>folderitem</xsl:text>
											</xsl:attribute>
											<xsl:attribute name="href">
												<xsl:call-template name="escape-apos">
                          <xsl:with-param name="string"
                            select = "concat($ApplPath, 'ItemViewer/', $DocNo, '?itemAdress=MyMusic/', substring-after(.,'MyMusic='))"
                          />
                        </xsl:call-template>
											</xsl:attribute>
											<xsl:attribute name="target">
												<xsl:text>_new</xsl:text>
											</xsl:attribute>
											<xsl:call-template name="escape-apos">
												<xsl:with-param name="string"
													select="substring-before(.,'MyMusic=')"/>
											</xsl:call-template>
										</a>
									</xsl:when>
                  <xsl:when test="contains(.,'MyVideo=')">
                    <a>
                      <xsl:attribute name="class">
                        <xsl:text>folderitem</xsl:text>
                      </xsl:attribute>
                      <xsl:attribute name="href">
                        <xsl:call-template name="escape-apos">
                          <xsl:with-param name="string"
                            select = "concat($ApplPath, 'ItemViewer/', $DocNo, '?itemAdress=MyVideo/', substring-after(.,'MyVideo='))"
                          />
                        </xsl:call-template>
                      </xsl:attribute>
                      <xsl:attribute name="target">
                        <xsl:text>_new</xsl:text>
                      </xsl:attribute>
                      <xsl:call-template name="escape-apos">
                        <xsl:with-param name="string"
													select="substring-before(.,'MyVideo=')"/>
                      </xsl:call-template>
                    </a>
                  </xsl:when>
                  <xsl:when test="contains(.,'RQItem=')">
										<a>
											<xsl:attribute name="class">
												<xsl:text>folderitem</xsl:text>
											</xsl:attribute>
											<xsl:attribute name="href">
												<xsl:call-template name="escape-apos">
													<xsl:with-param name="string"
														select="substring-after(.,'RQItem=')"/>
												</xsl:call-template>
											</xsl:attribute>
											<xsl:attribute name="target">
												<xsl:text>_new</xsl:text>
											</xsl:attribute>
											<xsl:call-template name="escape-apos">
												<xsl:with-param name="string"
													select="substring-before(.,'RQItem=')"/>
											</xsl:call-template>
										</a>
									</xsl:when>
									<xsl:otherwise>
										<xsl:call-template name="escape-apos">
											<xsl:with-param name="string" select="."/>
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
<!--      
			<xsl:apply-templates select="*[name() !='ID']" mode="item"/>
-->
      <xsl:apply-templates select="rqi:DocNo" mode="item"/>
      <xsl:apply-templates select="rqi:Title" mode="item"/>
      <xsl:apply-templates select="rqi:AuthorsEntity" mode="item"/>
      <xsl:apply-templates select="rqi:Institutions" mode="item"/>
      <xsl:apply-templates select="rqi:Source" mode="item"/>
      <xsl:apply-templates select="rqi:Edition" mode="item"/>
      <xsl:apply-templates select="rqi:ISDN" mode="item"/>
      <xsl:apply-templates select="rqi:Coden" mode="item"/>
      <xsl:apply-templates select="rqi:Volume" mode="item"/>
      <xsl:apply-templates select="rqi:Issue" mode="item"/>
      <xsl:apply-templates select="rqi:Pages" mode="item"/>
      <xsl:apply-templates select="rqi:Language" mode="item"/>
      <xsl:apply-templates select="rqi:Locality" mode="item"/>
      <xsl:apply-templates select="rqi:Publisher" mode="item"/>
      <xsl:apply-templates select="rqi:PublTime" mode="item"/>
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
    </table>
	</xsl:template>
	
	
	<xsl:template match="/">
    <table border="0" cellspacing="0" cellpadding="2">
			<xsl:apply-templates select="//rqi:RQItem"/>
		</table>
  </xsl:template>
	
	
	<xsl:template match="rqi:RQItem">
    <xsl:if test="rqi:AccessRights">
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
    </xsl:if>
		<tr>
			<td>
				<xsl:call-template name="RenderSingleView"/>
				<p/>
			</td>
		</tr>
	</xsl:template>
	
	
	<xsl:template match="rqi:RQItem/*[(name() !='file') and (string(.) !='') and (string(.) != ' ')]" mode="item">
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
	
	
	<xsl:template match="rqi:RQItem/rqi:Abstract">
		<table>
			<tr>
				<td class="comment">
					<xsl:choose>
						<xsl:when test="substring-after(.,'$$TOC$$=')">
							<xsl:call-template name="escape-apos">
								<xsl:with-param name="string"
									select="substring-before(.,'$$TOC$$=')"/>
							</xsl:call-template>
						</xsl:when>
						<xsl:otherwise>
							<xsl:call-template name="escape-apos">
								<xsl:with-param name="string" select="."/>
							</xsl:call-template>
						</xsl:otherwise>
					</xsl:choose>
				</td>
			</tr>
			<xsl:if test="substring-after(.,'$$TOC$$=')">
				<xsl:call-template name="generate-toc">
					<xsl:with-param name="tocstr" select="substring-after(.,'$$TOC$$=')"/>
          <xsl:with-param name="DocNo" select="../rqi:DocNo" />
				</xsl:call-template>
			</xsl:if>
		</table>
	</xsl:template>


  <xsl:template match="rqi:RQItem/rqi:AuthorsEntity">
    <xsl:apply-templates select="a:items/a:RQDescriptionComponent [bb:PersonDataSystem='unknown'] | a:items/a:RQDescriptionComponent [bb:PersonDataSystem='gnd']" />
  </xsl:template>


  <xsl:template match="a:items/a:RQDescriptionComponent [bb:PersonDataSystem='gnd']">
    <xsl:variable name="ID">
      <xsl:value-of select="generate-id()"/>
    </xsl:variable>
    <xsl:element name="span">
      <xsl:element name="a">
        <xsl:attribute name="href">
          <xsl:value-of select="bb:PersonID"/>
        </xsl:attribute>
        <xsl:attribute name="target">
          <xsl:text>_blank</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="title">
          <xsl:value-of select="'Die Beschreibung dieser Klasse anzeigen.'"/>
        </xsl:attribute>
        <xsl:value-of select="concat(bb:PersonalName, '; ')"/>
      </xsl:element>
      <xsl:value-of select="' - '"/>
      <xsl:value-of select="'('"/>
      <xsl:element name="span">
        <xsl:attribute name="id">
          <xsl:value-of select="$ID"/>
        </xsl:attribute>
        <xsl:element name="a">
          <xsl:attribute name="href">
            <!--<xsl:value-of
              select="concat('javascript:selectPredicates(null, &quot;',bb:PersonCode,'&quot;)')" />-->
          </xsl:attribute>
          <xsl:attribute name="id">
            <xsl:value-of select ="bb:PersonCode" />
          </xsl:attribute>
          <xsl:attribute name="class">
            <xsl:value-of select ="'select-predicates'" />
          </xsl:attribute>
          <xsl:attribute name="title">
            <xsl:value-of select="'Bezeichnung der Person suchen'"/>
          </xsl:attribute>
          <xsl:value-of select="'LD-Prädikate...'"/>
        </xsl:element>
      </xsl:element>
      <xsl:value-of select="')'"/>
      <xsl:value-of select="'; '"/>
    </xsl:element>
  </xsl:template>

  
  <xsl:template match="a:items/a:RQDescriptionComponent [bb:PersonDataSystem='unknown']">

    <xsl:value-of select="concat(bb:PersonalName, '; ')"/>
<!--
    <tr>
      <td class="fieldname" valign="top">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "name()" />
        </xsl:call-template>
      </td>
      <td class="fieldvalue">
        <xsl:variable name="Authorlist">
          <xsl:call-template name="ParseSubFields">
            <xsl:with-param name="FieldSubString" select="concat(bb:PersonalName,';')" />
          </xsl:call-template>
        </xsl:variable>

        <xsl:for-each select="msxsl:node-set($Authorlist)/SubFields[not(contains(.,'(=&gt;{Rolle})') or contains(.,'(=&gt; {Rolle})'))]">
          <xsl:if test="not(contains(following-sibling::*,'(=&gt;{Rolle})') or contains(following-sibling::*,'(=&gt; {Rolle})'))">
            <xsl:call-template name = "escape-apos">
              <xsl:with-param name = "string" select = "." />
            </xsl:call-template>
            <xsl:text>; </xsl:text>
          </xsl:if>
        </xsl:for-each>

        <table border="0" cellpadding="3" cellspacing="0" width="95%" align="right">
          <xsl:for-each select="msxsl:node-set($Authorlist)/SubFields[not(contains(.,'(=&gt;{Rolle})') or contains(.,'(=&gt; {Rolle})'))]">
            <xsl:if test="contains(following-sibling::*,'(=&gt;{Rolle})') or contains(following-sibling::*,'(=&gt; {Rolle})')">
              <tr>
                <td class="castfieldvalue">
                  <xsl:call-template name = "escape-apos">
                    <xsl:with-param name = "string" select = "." />
                  </xsl:call-template>
                </td>
                <td class="castfieldvalue">
                  <xsl:call-template name = "escape-apos">
                    <xsl:with-param name = "string" select = "substring-before(following-sibling::*,'(=')" />
                  </xsl:call-template>
                </td>
              </tr>
            </xsl:if>
          </xsl:for-each>
        </table>
      </td>
    </tr>
-->
  </xsl:template>

  
  <xsl:template match="rqi:RQItem/rqi:Classification">
		<xsl:text>RQC-Codes: </xsl:text>
		<xsl:apply-templates select="a:items/a:RQDescriptionComponent [b:ClassificationSystem='rq']"/>
		<xsl:if test="a:items/a:RQDescriptionComponent [b:ClassificationSystem='rvk']">
			<br/>
			<xsl:text>RVK-Codes: </xsl:text>
			<xsl:apply-templates select="a:items/a:RQDescriptionComponent [b:ClassificationSystem='rvk']"/>
		</xsl:if>
		<xsl:if test="a:items/a:RQDescriptionComponent [b:ClassificationSystem='ddc']">
			<br/>
			<xsl:text>DDC-Codes: </xsl:text>
			<xsl:apply-templates select="a:items/a:RQDescriptionComponent [b:ClassificationSystem='ddc']"/>
		</xsl:if>
		<xsl:if test="a:items/a:RQDescriptionComponent [b:ClassificationSystem='jel']">
			<br/>
			<xsl:text>JEL-Codes: </xsl:text>
			<xsl:apply-templates select="a:items/a:RQDescriptionComponent [b:ClassificationSystem='jel']"/>
		</xsl:if>
	</xsl:template>
	
	
	<xsl:template match="a:RQDescriptionComponent [b:ClassificationSystem='rq']">
    <xsl:element name="a">
      <xsl:attribute name="href">
        <xsl:value-of select="concat('javascript:call(&quot;','rqld/rqkos/rqc_',b:ClassCode,'&quot;)') "/>
      </xsl:attribute>
      <xsl:attribute name="title">
        <xsl:value-of select="'Die Beschreibung dieser Klasse anzeigen.'"/>
      </xsl:attribute>
      <xsl:value-of select="b:ClassCode"/>
    </xsl:element>
    <xsl:value-of select="' - '"/>
    <xsl:element name="a">
			<xsl:attribute name="href">
				<xsl:value-of select="concat('javascript:call(&quot;','rqkos/rqc_',b:ClassCode,'?d=',/rqi:RQItem/rqi:DocNo,'&quot;)')" />
			</xsl:attribute>
			<xsl:attribute name="title">
				<xsl:value-of select="'Alle Dokumente zu dieser Klasse anzeigen.'"/>
			</xsl:attribute>
			<xsl:value-of select="b:ClassShortTitle"/>
		</xsl:element>
		<xsl:value-of select="'; '"/>
	</xsl:template>
	
	
	<xsl:template match="a:RQDescriptionComponent [b:ClassificationSystem='rvk'] | a:RQDescriptionComponent [b:ClassificationSystem='jel'] | a:RQDescriptionComponent [b:ClassificationSystem='ddc']">
		<xsl:variable name="ID">
			<xsl:value-of select="generate-id()"/>
		</xsl:variable>

    <xsl:element name="span">
      <xsl:element name="a">
        <xsl:attribute name="href">
          <xsl:value-of select="b:ClassID"/>
        </xsl:attribute>
        <xsl:attribute name="target">
          <xsl:text>_blank</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="title">
          <xsl:value-of select="'Die Beschreibung dieser Klasse anzeigen.'"/>
        </xsl:attribute>
        <xsl:value-of select="b:ClassCode"/>
      </xsl:element>
      <xsl:value-of select="' - '"/>
        <xsl:choose>
          <xsl:when test="b:ClassShortTitle">
            <xsl:value-of select="b:ClassShortTitle"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="'('"/>
            <xsl:element name="span">
              <xsl:attribute name="id">
                <xsl:value-of select="$ID"/>
              </xsl:attribute>
              <xsl:element name="a">
                <xsl:attribute name="href">
                  <xsl:value-of
                    select="concat('javascript:callAjax(&quot;','rqitems/',/rqi:RQItem/rqi:DocNo,'/Classification/',count(preceding-sibling::a:RQDescriptionComponent ),'&quot;, &quot;',$ID,'&quot;)')" />
                </xsl:attribute>
                <xsl:attribute name="title">
                  <xsl:value-of select="'Bezeichnung der Klassen-Notation suchen'"/>
                </xsl:attribute>
                <xsl:value-of select="'more...'"/>
              </xsl:element>
            </xsl:element>
            <xsl:value-of select="')'"/>
          </xsl:otherwise>
        </xsl:choose>
			<xsl:value-of select="'; '"/>
		</xsl:element>
	</xsl:template>
	
	
	<!-- template provided with 2 typos by Jeni Tennison, 25 Feb 2001 -->
	<xsl:template name="escape-apos">
		<xsl:param name="string"/>
		
		<xsl:value-of select="$string" disable-output-escaping="no"/>
	</xsl:template>
	
</xsl:stylesheet>