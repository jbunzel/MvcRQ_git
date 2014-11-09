<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0" 	
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
  xmlns:ms="urn:schemas-microsoft-com:xslt" 
  xmlns:usr="urn:the-xml-files:xslt"
  xmlns:TransformUtils="urn:TransformHelper"
	xmlns:rqi="http://schemas.datacontract.org/2004/07/Mvc5RQ.Models"
	xmlns:b="http://schemas.datacontract.org/2004/07/RQLib.RQKos.Classifications" 
  xmlns:bb="http://schemas.datacontract.org/2004/07/RQLib.RQKos.Persons"              
	xmlns:a="http://schemas.datacontract.org/2004/07/RQLib.RQQueryResult.RQDescriptionElements">

	<xsl:output omit-xml-declaration="yes"/>

  <xsl:param name="ApplPath"/>
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
			<td class="rq-fieldvalue">
				<xsl:variable name="Signatures">
					<xsl:call-template name="ParseSubFields">
						<xsl:with-param name="FieldSubString" select="concat($tocstr,'; ')"/>
					</xsl:call-template>
				</xsl:variable>
				
				<table>
					<xsl:for-each select="msxsl:node-set($Signatures)/SubFields">
						<tr>
							<td class="rq-fieldvalue" valign="top">
								<xsl:choose>
									<xsl:when test="contains(.,'MyDoc=')">
										<a>
											<xsl:attribute name="class">
												<xsl:text>folderitem</xsl:text>
											</xsl:attribute>
											<xsl:attribute name="href">
												<xsl:call-template name="escape-apos">
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
    <xsl:element name="div">
      <xsl:element name="script">
        <xsl:attribute name="defer">
          <xsl:text>defer</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="type">
          <xsl:text>text/javascript</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="language">
          <xsl:text>JavaScript</xsl:text>
        </xsl:attribute>
        <xsl:text>
          <![CDATA[
          var thumbNail;
       
          function checkAvail_OLD(id, isbn) {
            thumbNail = id;
            debugger;
            isbn = "0064430073, 0064430065, 0606018034, 0606011234";
            var div = document.getElementById(id);
            if (div.firstChild) div.removeChild(div.firstChild);
            
            // Show a "Loading..." indicator.
            //var div = document.getElementById(id);
            var p = document.createElement('p');
            p.appendChild(document.createTextNode('Loading...'));
            div.appendChild(p);

            // Delete any previous Google Booksearch JSON queries.
            var jsonScript = document.getElementById("GoogleBooksAPIScript");
            if (jsonScript) {
              jsonScript.parentNode.removeChild(GoogleBooksAPIScript);
            }

            // Add a script element with the src as the user's Google Booksearch query.
            // JSON output is specified by including the alt=json-in-script argument
            // and the callback funtion is also specified as a URI argument.
            //var scriptElement = document.createElement("script");
            //scriptElement.setAttribute("id", "GoogleBooksAPIScript");
            //scriptElement.setAttribute("src", "http://books.google.com/books?bibkeys=" + escape(isbn) + "&jscmd=viewapi&callback=showThumbNail");
            //scriptElement.setAttribute("type", "text/javascript");
  
            // make the request to Google booksearch
            //document.documentElement.firstChild.appendChild(scriptElement);
            
            $.ajax({
                //url: "http://books.google.com/books?bibkeys=ISBN:0451526538&jscmd=viewapi&callback=showThumbNail",
                url: "https://openlibrary.org/api/books?bibkeys=ISBN:0451526538",                
                headers: { 
                  Accept : "json"
                },
                type: "GET",
                data: null,
                //dataType: "text/javascript",
                success: function (data) {
                    debugger;
                    showThumbNail(data);
                },
                error: function (xhr) {
                    debugger;
                    alert("Error:  " +xhr); 
                }
            });
          }
]]>
      </xsl:text>
      </xsl:element>
      <table border="0" cellspacing="0" cellpadding="2">
        <xsl:apply-templates select="//rqi:RQItem"/>
      </table>
    </xsl:element>
  </xsl:template>
	
	
	<xsl:template match="rqi:RQItem">
    <xsl:if test="rqi:AccessRights">
      <tr>
        <td>
          <xsl:element name="div">
            <xsl:attribute name="class">
              <xsl:value-of select="'rq-action-bar'"/>
            </xsl:attribute>
            <xsl:variable name="rights" select="substring-after(rqi:AccessRights, 'actual=')" />
            <xsl:if test="contains($rights, 'edit')">
              <xsl:element name="a">
                <xsl:attribute name="href">
                  <xsl:value-of select="concat('javascript:call(&quot;rqitems/edit/',rqi:DocNo,'&quot;)')" />
                </xsl:attribute>
                <xsl:attribute name="class">
                  <xsl:value-of select="''" />
                </xsl:attribute>
                <xsl:element name="i">
                  <xsl:attribute name="class">
                    <xsl:value-of select="'fa fa-edit'"/>
                  </xsl:attribute>
                </xsl:element>
                <xsl:text> edit</xsl:text>
              </xsl:element>
            </xsl:if>
            <xsl:if test="contains($rights, 'copy')">
              <xsl:text> | </xsl:text>
              <xsl:element name="a">
                <xsl:attribute name="href">
                  <xsl:value-of select="concat('javascript:call(&quot;rqitems/copy/',rqi:DocNo,'&quot;)')" />
                </xsl:attribute>
                <xsl:attribute name="class">
                  <xsl:value-of select="''" />
                </xsl:attribute>
                <xsl:element name="i">
                  <xsl:attribute name="class">
                    <xsl:value-of select="'fa fa-copy'"/>
                  </xsl:attribute>
                </xsl:element>
                <xsl:text> copy</xsl:text>
              </xsl:element>
            </xsl:if>
            <xsl:if test="contains($rights, 'delete')">
              <xsl:text> | </xsl:text>
              <xsl:element name="a">
                <xsl:attribute name="href">
                  <xsl:value-of select="concat('javascript:call(&quot;rqitems/delete/',rqi:DocNo,'&quot;)')" />
                </xsl:attribute>
                <xsl:attribute name="class">
                  <xsl:value-of select="''" />
                </xsl:attribute>
                <xsl:element name="i">
                  <xsl:attribute name="class">
                    <xsl:value-of select="'fa fa-trash-o'"/>
                  </xsl:attribute>
                </xsl:element>
                <xsl:text> delete</xsl:text>
              </xsl:element>
            </xsl:if>
          </xsl:element>
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
			<td class="rq-fieldname" valign="top">
				<xsl:call-template name="escape-apos">
					<xsl:with-param name="string" select="name()"/>
				</xsl:call-template>
			</td>
			<td class="rq-fieldvalue">
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


  <xsl:template match="rqi:RQItem/rqi:AuthorsEntity" mode="item">
    <xsl:if test="(string(//a:items//bb:PersonalName)  != '') and (string(//a:items//bb:PersonalName) != ' ')">
      <tr>
        <td class="rq-fieldname" valign="top">
          <xsl:call-template name="escape-apos">
            <xsl:with-param name="string" select="'Authors'"/>
          </xsl:call-template>
        </td>
        <td class="rq-fieldvalue">
          <xsl:for-each select="a:items//bb:PersonalName[not(contains(.,'(=&gt;{Rolle})') or contains(.,'(=&gt; {Rolle})'))]">
            <xsl:apply-templates select="ancestor::a:RQDescriptionComponent [bb:PersonDataSystem='unknown'] | ancestor::a:RQDescriptionComponent [bb:PersonDataSystem='gnd']" />
          </xsl:for-each>
          <xsl:element name="table">
            <xsl:attribute name="class">
              <xsl:text>rq-cast-table</xsl:text>
            </xsl:attribute>
            <xsl:element name="tbody">
              <xsl:for-each select="a:items//bb:PersonalName[contains(.,'(=&gt;{Rolle})') or contains(.,'(=&gt; {Rolle})')]">
                <xsl:apply-templates select="ancestor::a:RQDescriptionComponent [bb:PersonDataSystem='unknown'] | ancestor::a:RQDescriptionComponent [bb:PersonDataSystem='gnd']" mode="cast-table" />
              </xsl:for-each>
            </xsl:element>
          </xsl:element>
        </td>
      </tr>
    </xsl:if>
  </xsl:template>

  
  <xsl:template match="a:items/a:RQDescriptionComponent [bb:PersonDataSystem='gnd']" mode="cast-table">
    
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

  
  <xsl:template match="a:items/a:RQDescriptionComponent [bb:PersonDataSystem='unknown']" mode="cast-table">
    <tr>
      <td class="rq-castfielname">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "substring-before(bb:PersonalName, '; ')" />
        </xsl:call-template>
      </td>
      <td class="rq-castfieldvalue">
        <xsl:variable name="castname">
          <xsl:choose>
            <xsl:when test="contains(bb:PersonalName, '&lt;')">
              <xsl:value-of select = "substring-before(substring-after(bb:PersonalName, '; '), '&lt;')" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select ="substring-before(substring-after(bb:PersonalName, '; '), ' (')"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:variable>

        <xsl:call-template name = "escape-apos">
            <xsl:with-param name = "string" select = "$castname" />
        </xsl:call-template>
      </td>
    </tr>
  </xsl:template>

  
  <xsl:template match="a:items/a:RQDescriptionComponent [bb:PersonDataSystem='unknown']">
    <xsl:call-template name = "escape-apos">
      <xsl:with-param name = "string" select = "concat(bb:PersonalName, '; ')" />
    </xsl:call-template>
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

  
  <xsl:template match="rqi:RQItem/rqi:Institutions[(string(.) !='') and (string(.) != ' ')]" mode="item">
    <tr>
      <td class="rq-fieldname" valign="top">
        <xsl:call-template name="escape-apos">
          <xsl:with-param name="string" select="name()"/>
        </xsl:call-template>
      </td>
      <td class="rq-fieldvalue">
        <xsl:variable name="InstitutionList">
          <xsl:call-template name="ParseSubFields">
            <xsl:with-param name="FieldSubString" select="concat(.,';')" />
          </xsl:call-template>
        </xsl:variable>

        <xsl:for-each select="msxsl:node-set($InstitutionList)/SubFields[not(contains(.,'(=&gt;{Rolle})') or contains(.,'(=&gt; {Rolle})'))]">
          <xsl:if test="not(contains(following-sibling::*,'(=&gt;{Rolle})') or contains(following-sibling::*,'(=&gt; {Rolle})'))">
            <xsl:value-of select = "." />
            <xsl:text>; </xsl:text>
          </xsl:if>
        </xsl:for-each>

        <xsl:element name="table">
          <xsl:for-each select="msxsl:node-set($InstitutionList)/SubFields[not(contains(.,'(=&gt;{Rolle})') or contains(.,'(=&gt; {Rolle})'))]">
            <xsl:if test="contains(following-sibling::*,'(=&gt;{Rolle})') or contains(following-sibling::*,'(=&gt; {Rolle})')">
              <tr>
                <td class="rq-castfieldname">
                  <xsl:call-template name = "escape-apos">
                    <xsl:with-param name = "string" select = "." />
                  </xsl:call-template>
                </td>
                <td class="rq-castfieldvalue">
                  <xsl:call-template name = "escape-apos">
                    <xsl:with-param name = "string" select = "substring-before(following-sibling::*,'(=')" />
                  </xsl:call-template>
                </td>
              </tr>
            </xsl:if>
          </xsl:for-each>
        </xsl:element>
      </td>
    </tr>
  </xsl:template>

  
  <xsl:template match="rqi:RQItem/rqi:Signature" mode="item">
    <xsl:variable name="DocNo" select="../rqi:DocNo" />
    
    <xsl:variable name="NormISDN">
      <xsl:if test="../rqi:ISDN[(string(.) !='') and (string(.) != ' ')]">
<!--        
        <xsl:value-of select="usr:NormalizeISDN(string(../rqi:ISDN))" />
-->        
        <xsl:value-of select="TransformUtils:NormalizeISDN(string(../rqi:ISDN))" />
      </xsl:if>
    </xsl:variable>

    <xsl:variable name="EID">
      <xsl:value-of select="generate-id()"/>
    </xsl:variable>

    <tr>
      <td class="rq-fieldname" valign="top">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "'Availability'" />
        </xsl:call-template>
      </td>
      <td class="rq-fieldvalue">
        <xsl:variable name="IdNr" select="../ID" />
        <xsl:variable name="Signatures">
          <xsl:call-template name="ParseSubFields">
            <xsl:with-param name="FieldSubString" select="concat(.,';')" />
          </xsl:call-template>
        </xsl:variable>
        <table>
          <!--          
          <xsl:if test="../rqi:ISDN[(string(.) !='') and (string(.) != ' ')]">
-->
          <xsl:if test="((string($NormISDN) !='') and (string($NormISDN) != ' '))">     
            <tr>
              <td class="fieldvalue">
                <xsl:element name="div">
                  <xsl:attribute name="ID">
                    <xsl:value-of select="$EID" />
                  </xsl:attribute>
                  <xsl:attribute name="Name">
                    <xsl:value-of select="$EID" />
                  </xsl:attribute>
                  <xsl:attribute name="style">
                    <xsl:text>display : block; border : 1px;</xsl:text>
                  </xsl:attribute>
                </xsl:element>
              </td>
            </tr>
            <xsl:element name="script">
              <xsl:attribute name="defer">
                <xsl:text>defer</xsl:text>
              </xsl:attribute>
              <xsl:attribute name="type">
                <xsl:text>text/javascript</xsl:text>
              </xsl:attribute>
              <xsl:attribute name="language">
                <xsl:text>JavaScript</xsl:text>
              </xsl:attribute>
              <xsl:text>checkAvail('</xsl:text>
              <xsl:value-of select="$EID" />
              <xsl:text>','</xsl:text>
              <xsl:value-of select="$NormISDN" />
              <xsl:text>');</xsl:text>
            </xsl:element>
          </xsl:if>
          <xsl:for-each select="msxsl:node-set($Signatures)/SubFields">
            <tr>
              <td class="fieldvalue" valign="top">
                <xsl:choose>
                  <xsl:when test="contains(.,'MyDoc=')">
                    <a title="Read Online">
                      <xsl:attribute name="class">
                        <xsl:text>rq-action-bar</xsl:text>
                      </xsl:attribute>
                      <xsl:attribute name="href">
                        <xsl:call-template name = "escape-apos">
                          <xsl:with-param name="string" select = "concat($ApplPath, 'ItemViewer/', $DocNo, '?itemAdress=MyDocs/', substring-after(.,'MyDoc='))" />
                        </xsl:call-template>
                      </xsl:attribute>
                      <xsl:attribute name="target">
                        <xsl:text>_new</xsl:text>
                      </xsl:attribute>
                      <i>
                        <xsl:attribute name="class">
                          <xsl:value-of select="'fa fa-eye fa-2x'" />
                        </xsl:attribute>
                        <!--<xsl:attribute name="src">
                          <xsl:value-of select="concat($ApplPath,'Images/RQFTCollection.jpg')" />
                        </xsl:attribute>
                        <xsl:attribute name="align">absmiddle</xsl:attribute>
                        <xsl:attribute name="border">0</xsl:attribute>
                        <xsl:attribute name="hspace">5</xsl:attribute>-->
                      </i>
                      <xsl:text> RiQuest Online Reader</xsl:text>
                    </a>
                  </xsl:when>
                  <xsl:when test="contains(.,'MyVideo=')">
                    <a title="View Online">
                      <xsl:attribute name="class">
                        <xsl:text>folderitem</xsl:text>
                      </xsl:attribute>
                      <xsl:attribute name="href">
                        <xsl:call-template name = "escape-apos">
                          <xsl:with-param name="string" select = "concat($ApplPath, 'ItemViewer/', $DocNo, '?itemAdress=MyVideo/', substring-after(.,'MyVideo='))" />
                        </xsl:call-template>
                      </xsl:attribute>
                      <xsl:attribute name="target">
                        <xsl:text>_new</xsl:text>
                      </xsl:attribute>
                      <i>
                        <xsl:attribute name="class">
                          <xsl:value-of select="'fa fa-film fa-2x'" />
                        </xsl:attribute>
                        <!--<img title="View Online">
                        <xsl:attribute name="src">
                          <xsl:value-of select="concat($ApplPath,'Images/RQOVCollection.jpg')" />
                        </xsl:attribute>
                        <xsl:attribute name="align">absmiddle</xsl:attribute>
                        <xsl:attribute name="border">0</xsl:attribute>
                        <xsl:attribute name="hspace">5</xsl:attribute>
                      </img>-->
                      </i>
                      <xsl:text>  RiQuest Online Video </xsl:text>
                    </a>
                  </xsl:when>
                  <xsl:when test="contains(.,'MyMusic=')">
                    <a title="Listen Online">
                      <xsl:attribute name="class">
                        <xsl:text>folderitem</xsl:text>
                      </xsl:attribute>
                      <xsl:attribute name="href">
                        <xsl:call-template name = "escape-apos">
                          <xsl:with-param name="string" select = "concat($ApplPath, 'ItemViewer/', $DocNo, '?itemAdress=MyMusic/', substring-after(.,'MyMusic='))" />
                        </xsl:call-template>
                      </xsl:attribute>
                      <xsl:attribute name="target">
                        <xsl:text>_new</xsl:text>
                      </xsl:attribute>
                      <i>
                        <xsl:attribute name="class">
                          <xsl:value-of select="'fa fa-volume-up fa-2x'" />
                        </xsl:attribute>
                        <!--<img title="Listen Online">
                        <xsl:attribute name="src">
                          <xsl:value-of select="concat($ApplPath,'Images/RQOACollection.jpg')" />
                        </xsl:attribute>
                        <xsl:attribute name="align">absmiddle</xsl:attribute>
                        <xsl:attribute name="border">0</xsl:attribute>
                        <xsl:attribute name="hspace">5</xsl:attribute>
                      </img>-->
                      </i>
                      <xsl:text>  RiQuest Online Audio</xsl:text>
                    </a>
                  </xsl:when>
                  <xsl:when test="contains(.,'https://proxy.nationallizenzen.de/login?url=')">
<!--                    
                    <xsl:text>DFG-Nationallizenz: </xsl:text>
-->
                    <a title="Externe Quelle (Nationallizenz)">
                      <xsl:attribute name="class">
                        <xsl:text>folderitem</xsl:text>
                      </xsl:attribute>
                      <xsl:attribute name="href">
                        <xsl:call-template name = "escape-apos">
                          <xsl:with-param name = "string" select = "concat('https://',substring-after(.,'https://'))" />
                        </xsl:call-template>
                      </xsl:attribute>
                      <xsl:attribute name="target">
                        <xsl:text>_new</xsl:text>
                      </xsl:attribute>
                      <i>
                        <xsl:attribute name="class">
                          <xsl:value-of select="'fa fa-sign-out'" />
                        </xsl:attribute>
                        <!--<img title="Externe Quelle (Nationallizenz)">
                        <xsl:attribute name="src">
                          <xsl:value-of select="concat($ApplPath,'Images/RQNLCollection.jpg')" />
                        </xsl:attribute>
                        <xsl:attribute name="align">absmiddle</xsl:attribute>
                        <xsl:attribute name="border">0</xsl:attribute>
                        <xsl:attribute name="hspace">5</xsl:attribute>
                      </img>-->
                      </i>
                      <!--
                      <xsl:call-template name = "escape-apos">
                        <xsl:with-param name = "string" select = "concat('https://',substring-after(.,'https://'))" />
                      </xsl:call-template>
-->
                      <xsl:text>  Externe Quelle (DFG-Nationallizenz)</xsl:text>
                    </a>
                  </xsl:when>
                  <xsl:when test="contains(.,'http://')">
<!--                    
                    <xsl:text>extern: </xsl:text>
-->                    
                    <a>
                      <xsl:attribute name="class">
                        <xsl:text>folderitem</xsl:text>
                      </xsl:attribute>
                      <xsl:attribute name="href">
                        <xsl:call-template name = "escape-apos">
                          <xsl:with-param name = "string" select = "concat('http://',substring-after(.,'http://'))" />
                        </xsl:call-template>
                      </xsl:attribute>
                      <xsl:attribute name="target">
                        <xsl:text>_new</xsl:text>
                      </xsl:attribute>
                      <i>
                        <xsl:attribute name="class">
                          <xsl:value-of select="'fa fa-sign-out'" />
                        </xsl:attribute>
                        <!--<img title="Externe Quelle">
                        <xsl:attribute name="src">
                          <xsl:value-of select="concat($ApplPath,'Images/RQEXCollection.jpg')" />
                        </xsl:attribute>
                        <xsl:attribute name="align">absmiddle</xsl:attribute>
                        <xsl:attribute name="border">0</xsl:attribute>
                        <xsl:attribute name="hspace">5</xsl:attribute>
                      </img>-->
                      </i>
                      <xsl:text>  Externe Quelle</xsl:text>
<!--                      
                      <xsl:call-template name = "escape-apos">
                        <xsl:with-param name = "string" select = "concat('http://',substring-after(.,'http://'))" />
                      </xsl:call-template>
-->                      
                    </a>
                  </xsl:when>
                  <xsl:when test="contains(.,'rtsp://')">
                    <xsl:text>extern: </xsl:text>
                    <a>
                      <xsl:attribute name="class">
                        <xsl:text>folderitem</xsl:text>
                      </xsl:attribute>
                      <xsl:attribute name="href">
                        <xsl:call-template name = "escape-apos">
                          <xsl:with-param name = "string" select = "concat('rtsp://',substring-after(.,'rtsp://'))" />
                        </xsl:call-template>
                      </xsl:attribute>
                      <xsl:attribute name="target">
                        <xsl:text>_new</xsl:text>
                      </xsl:attribute>
                      <xsl:call-template name = "escape-apos">
                        <xsl:with-param name = "string" select = "concat('rtsp://',substring-after(.,'rtsp://'))" />
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
  

  <ms:script language="JavaScript" implements-prefix="usr">
    <![CDATA[
        function NormalizeISDN(ops) {
					var result = "";
					var isbnList = ops.split(";")
					
					for (var i=0; i < isbnList.length; i++) {
					   isbnList[i] = isbnList[i].replace(/is.n./i, "");
					   isbnList[i] = isbnList[i].replace(/ /g, "");
						 isbnList[i] = isbnList[i].replace(/-/g, "");
						 if ((isbnList[i].length == 10) || (isbnList[i].length == 13)) {
						   result += isbnList[i] + " ";
						 }
					}
					//return result;
					//return ops.replace(/-/g, "");
          
          var reISBN=/(ISBN[\:\=\s][\s]*(?=[-0-9xX ]{13})(?:[0-9]+[- ]){3}[0-9]*[xX0-9])|(ISBN[\:\=\s][ ]*\d{9,10}[\d|x])/g;
          var strMatches = ops.match(reISBN);
          if (strMatches == null) return "";
          // now output matches into report document
          for (var j = 0; j < strMatches.length; j++) {
            result = result + strMatches[j] + " "
            }
          return result;
				}
    ]]>
  </ms:script>


    <!-- template provided with 2 typos by Jeni Tennison, 25 Feb 2001 -->
	<xsl:template name="escape-apos">
		<xsl:param name="string"/>
		
		<xsl:value-of select="$string" disable-output-escaping="no"/>
	</xsl:template>
	
</xsl:stylesheet>