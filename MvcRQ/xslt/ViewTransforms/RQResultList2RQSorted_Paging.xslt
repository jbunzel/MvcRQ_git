<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
	              xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:mo="http://www.loc.gov/mods/v3">

  <xsl:param name="ApplPath" select="''" />
  <xsl:param name="MyDocsPath" select="''" />
  <xsl:param name="MyVideoPath" select="''" />
  <xsl:param name="MyMusicPath" select="''" />
  <xsl:param name="SortType" select="'Regal'" />


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


  <xsl:template name="substring-before-last">
		<xsl:param name="sourcestring"/>
		<xsl:param name="teststring"/>
		<xsl:choose>
			<xsl:when test="contains($sourcestring, $teststring)">
				<xsl:call-template name="substring-before-last">
					<xsl:with-param name="sourcestring" select="substring-after($sourcestring, $teststring)" />
					<xsl:with-param name="teststring" select="$teststring" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$sourcestring" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>


  <xsl:template name="selectfirstclass">
    <xsl:param name="list"/>
    <xsl:param name="classes"/>

    <xsl:variable name="wlist" select="concat(';',$list)"/>
    <xsl:choose>
      <xsl:when test="string-length($classes) > 0">
        <xsl:variable name="first" select="normalize-space(substring-before($classes, ';'))"/>
        <xsl:variable name="rest" select="normalize-space(substring-after($classes, ';'))"/>
        <xsl:choose>
          <xsl:when test="substring($wlist,1,string-length(concat(';',$first)))=concat(';',$first)">
            <xsl:variable name="v1" select="concat($first,substring-after($wlist,concat(';',$first)))"/>

            <xsl:value-of select="substring-before($v1,';')"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name="selectfirstclass">
              <xsl:with-param name="list" select="$list"/>
              <xsl:with-param name="classes" select="$rest"/>
            </xsl:call-template>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$list"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <xsl:template name="selectclass">
		<!-- 
			selects the first substring of the semicolon seperated list of strings $list, 
			that starts with any of the substrings contained in the semicolon seperated 
			list of strings in $classes
			
			$list is the list of full length RVK classification codes of the current element
			
			$classes is the list of leading characters of the RVK classification codes
			corresponding to the current class of MyClassification
			
			if there is no match $list is returned    
		-->
		<xsl:param name="list"/>
		<xsl:param name="classes"/>
		
		<xsl:variable name="wlist" select="concat(';',$list)"/>
		<xsl:choose>
			<xsl:when test="string-length($classes) > 0">
				<xsl:variable name="first" select="normalize-space(substring-before($classes, ';'))"/>
				<xsl:variable name="rest" select="normalize-space(substring-after($classes, ';'))"/>
				<xsl:choose>
					<xsl:when test="contains($wlist,concat(';',$first))">
						<xsl:variable name="v1"
							select="concat($first,substring-after($wlist,concat(';',$first)))"/>
						
						<xsl:value-of select="substring-before($v1,';')"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="selectclass">
							<xsl:with-param name="list" select="$list"/>
							<xsl:with-param name="classes" select="$rest"/>
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$list"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>


  <xsl:template name="generate-st">
    <xsl:call-template name="generate-title" >
      <xsl:with-param name="title" select="preceding-sibling::Title" />
    </xsl:call-template>
    <xsl:value-of select="'&amp;AT='" />
    <xsl:call-template name="generate-author" >
      <xsl:with-param name="authors" select="preceding-sibling::Authors" />
    </xsl:call-template>
    <xsl:value-of select="'&amp;LC='" />
    <xsl:call-template name="generate-locality" >
      <xsl:with-param name="locality" select="preceding-sibling::Locality" />
    </xsl:call-template>
  </xsl:template>
  

  <xsl:template name="generate-title">
    <xsl:param name="title" select="."/>

    <xsl:choose>
      <xsl:when test="contains($title,'|')">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "normalize-space(substring-before($title,'|'))" />
        </xsl:call-template>
      </xsl:when>
      <xsl:when test="contains($title,'=')">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "normalize-space(substring-before($title,'='))" />
        </xsl:call-template>
      </xsl:when>
      <xsl:when test="contains($title,':')">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "normalize-space(substring-before($title,':'))" />
        </xsl:call-template>
      </xsl:when>
      <xsl:when test="contains($title,'/')">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "normalize-space(substring-before($title,'/'))" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "$title" />
        </xsl:call-template>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <xsl:template name="generate-author">
    <xsl:param name="authors" select="." />

    <xsl:choose>
      <xsl:when test="(string(.) !='') and (string(.) != ' ')">
        <xsl:choose>
          <xsl:when test="string-length($authors) &lt;= 40">
            <xsl:call-template name = "escape-apos">
              <xsl:with-param name = "string" select = "$authors" />
            </xsl:call-template>
          </xsl:when>
          <xsl:when test="contains(substring($authors,30),';')">
            <xsl:call-template name = "escape-apos">
              <xsl:with-param name = "string" select = "normalize-space(concat(substring($authors,1,39),substring-before(substring($authors,40),';'),' [u.a.]'))" />
            </xsl:call-template>
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name = "escape-apos">
              <xsl:with-param name = "string" select = "$authors" />
            </xsl:call-template>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name="generate-institution">
          <xsl:with-param name="institutions" select="preceding-sibling::Institutions" />
        </xsl:call-template>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <xsl:template name="generate-institution">
    <xsl:param name="institutions" select="."/>

    <xsl:if test="(string($institutions) !='') and (string($institutions) != ' ')">
      <xsl:choose>
        <xsl:when test="contains($institutions,';')">
          <xsl:call-template name = "escape-apos">
            <xsl:with-param name = "string" select = "normalize-space(substring-before($institutions,';'))" />
          </xsl:call-template>
        </xsl:when>
        <xsl:otherwise>
          <xsl:call-template name = "escape-apos">
            <xsl:with-param name = "string" select = "$institutions" />
          </xsl:call-template>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
  </xsl:template>


  <xsl:template name="generate-locality">
    <xsl:param name="locality" select="." />

    <xsl:if test="(string($locality) !='') and (string($locality) != ' ')">
      <xsl:choose>
        <xsl:when test="substring($locality,string-length($locality)) = ';'">
          <xsl:call-template name = "escape-apos">
            <xsl:with-param name = "string" select = "normalize-space(substring($locality,1,string-length($locality)-1))" />
          </xsl:call-template>
        </xsl:when>
        <xsl:otherwise>
          <xsl:call-template name = "escape-apos">
            <xsl:with-param name = "string" select = "$locality" />
          </xsl:call-template>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:choose>
        <xsl:when test="(string($locality/../Publisher) !='') and (string($locality/../Publisher) != ' ')">
          <xsl:text> : </xsl:text>
        </xsl:when>
        <xsl:when test="(string($locality/../PublTime) !='') and (string($locality/../PublTime) != ' ')">
          <xsl:text>; </xsl:text>
        </xsl:when>
      </xsl:choose>
    </xsl:if>

    <xsl:if test="(string($locality/../Publisher) !='') and (string($locality/../Publisher) != ' ')">
      <xsl:choose>
        <xsl:when test="substring($locality/../Publisher,string-length($locality/../Publisher)) = ';'">
          <xsl:call-template name = "escape-apos">
            <xsl:with-param name = "string" select = "normalize-space(substring($locality/../Publisher,1,string-length($locality/../Publisher)-1))" />
          </xsl:call-template>
        </xsl:when>
        <xsl:otherwise>
          <xsl:call-template name = "escape-apos">
            <xsl:with-param name = "string" select = "$locality/../Publisher" />
          </xsl:call-template>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:if test="(string($locality/../PublTime) !='') and (string($locality/../PublTime) != ' ')">
        <xsl:text> ; </xsl:text>
      </xsl:if>
    </xsl:if>

    <xsl:if test="(string($locality/../PublTime) !='') and (string($locality/../PublTime) != ' ')">
      <xsl:choose>
        <xsl:when test="substring($locality/../PublTime,string-length($locality/../PublTime)) = ';'">
          <xsl:call-template name = "escape-apos">
            <xsl:with-param name = "string" select = "normalize-space(substring($locality/../PublTime,1,string-length($locality/../PublTime)-1))" />
          </xsl:call-template>
        </xsl:when>
        <xsl:otherwise>
          <xsl:call-template name = "escape-apos">
            <xsl:with-param name = "string" select = "$locality/../PublTime" />
          </xsl:call-template>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
  </xsl:template>


  <xsl:template name="getitems-by-shelf">
    <xsl:param name="DocNodeSet" select="''"/>
    <xsl:param name="ShelfType" select="''" />

    <xsl:for-each select="/RQResultList/Systematiken/Systematik">
      <xsl:sort select="DDCNumber"/>

      <xsl:variable name="class" select="DDCNumber"/>
      <xsl:if test="count($DocNodeSet[starts-with(Classification, $class)]) > 0">

        <xsl:variable name="PID" select="ID"/>
        <xsl:variable name="regsigns" select="RegensburgSign"/>

        <Systematik>
          <Description>
            <xsl:value-of select="concat($ShelfType, ' (', DDCNumber, ' ', Description, ')' )"/>
          </Description>
          <DDCNumber>
            <xsl:text>BU</xsl:text>
          </DDCNumber>
          <ClassPath>
            <xsl:value-of select="concat('BU=', $ShelfType, ' (', RegensburgDesc, ')')"/>
          </ClassPath>
          <Hits>
            <xsl:value-of select="count($DocNodeSet[starts-with(Classification, $class)])"/>
          </Hits>

          <!--
	        sortkey are generated and elements are written to a node-set variable
		      before they are written to the output in sort order
-->
          <xsl:variable name="DocList">
            <xsl:for-each select="$DocNodeSet[starts-with(Classification, $class)]">

              <xsl:variable name="sortkey">
                <xsl:choose>
                  <xsl:when test="starts-with(Feld30,'ZZ999')">
                    <xsl:value-of select="substring-before(concat($class, substring-after(Classification,$class), ';'), ';')" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:call-template name="selectfirstclass">
                      <xsl:with-param name="list" select="Feld30"/>
                      <xsl:with-param name="classes" select="$regsigns"/>
                    </xsl:call-template>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:variable>

              <Dokument>
                <SortOrder>
                  <xsl:call-template name="substring-before-last">
                    <xsl:with-param name="sourcestring" select="concat(substring-before(Classification,concat('; §§RVK§§:',$sortkey)),'0',$sortkey)" />
                    <xsl:with-param name="teststring" select="'; '" />
                  </xsl:call-template>
                </SortOrder>
                <xsl:copy-of select="./*"/>
              </Dokument>
            </xsl:for-each>
          </xsl:variable>

          <xsl:for-each select="msxsl:node-set($DocList)/Dokument">
            <xsl:sort select="SortOrder"/>
            <xsl:sort select="CreateTime" order="ascending"/>

            <xsl:copy-of select="."/>
          </xsl:for-each>
        </Systematik>
      </xsl:if>
    </xsl:for-each>
    <xsl:if
      test="count($DocNodeSet[not(Classification) or (string-length(normalize-space(Classification))=0) or starts-with(classification,'§')])">

      <Systematik>
        <Description>
          <xsl:value-of select="concat($ShelfType, ' (Z nicht klassifiziert)')"/>
        </Description>
        <DDCNumber>
          <xsl:text>BU</xsl:text>
        </DDCNumber>
        <ClassPath>
          <xsl:value-of select="concat('BU=', $ShelfType, ' (Z nicht klassifiziert)')"/>
        </ClassPath>
        <Hits>
          <xsl:value-of
            select="count($DocNodeSet[not(Classification) or (string-length(Classification)=0) or starts-with(classification,'§')])"
          />
        </Hits>
        <xsl:for-each
          select="$DocNodeSet[not(Classification) or (string-length(Classification)=0) or starts-with(classification,'§')]">
          <xsl:sort select="parent::*/Feld30 | @Feld30"/>
          <xsl:choose>
            <xsl:when test="not(./Classification)">
              <Dokument>
                <SortOrder>
                  <xsl:value-of select="./Feld30"/>
                </SortOrder>
                <xsl:copy-of select="./*"/>
                <Classification>
                  <xsl:text></xsl:text>
                </Classification>
              </Dokument>
            </xsl:when>
            <xsl:otherwise>
              <Dokument>
                <SortOrder>
                  <xsl:value-of select="./Feld30"/>
                </SortOrder>
                <xsl:copy-of select="./*"/>
              </Dokument>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>
      </Systematik>
    </xsl:if>
  </xsl:template>
  
  
  <xsl:template name="getitems-by-class">
		<xsl:param name="class" select="classname"/>
		<xsl:param name="DocNodeSet" select="//Classification"/>
		<xsl:param name="DirNodeSet" select="//notation"/>
		<xsl:param name="Desc" select="Description"/>
		<xsl:param name="DDC" select="DDCNumber"/>

		<xsl:if
			test="$DocNodeSet | $DirNodeSet/parent::folder[name(parent::node())!='folder' and name(parent::node())!='filelist'] | $DirNodeSet/parent::file[name(parent::node())!='folder' and name(parent::node())!='filelist']">
			<xsl:variable name="regsigns" select="RegensburgSign"/>

			<Systematik>
				<Description>
					<xsl:value-of select="$Desc"/>
				</Description>
				<DDCNumber>
					<xsl:value-of select="$DDC"/>
				</DDCNumber>
				<ClassPath>
					<xsl:value-of select="RegensburgDesc"/>
				</ClassPath>
				<Hits>
					<xsl:value-of
						select="count($DocNodeSet)+count($DirNodeSet/parent::folder[name(parent::node())!='folder' and name(parent::node())!='filelist'])+count($DirNodeSet/parent::file[name(parent::node())!='folder' and name(parent::node())!='filelist'])"
					/>
				</Hits>
				<xsl:variable name="DocList">
					<xsl:for-each
						select="$DocNodeSet | $DirNodeSet/parent::folder[name(parent::node())!='folder' and name(parent::node())!='filelist'] | $DirNodeSet/parent::file[name(parent::node())!='folder' and name(parent::node())!='filelist']">

						<xsl:variable name="sortkey">
							<xsl:call-template name="selectclass">
								<xsl:with-param name="list" select="parent::*/Feld30 | @Feld30"/>
								<xsl:with-param name="classes" select="$regsigns"/>
							</xsl:call-template>
						</xsl:variable>

						<xsl:choose>
							<xsl:when test="name(.)='Classification'">
								<Dokument>
									<SortOrder>
										<xsl:call-template name="substring-before-last">
											<xsl:with-param name="sourcestring"
												select="concat(substring-before(parent::*/Classification,concat('; §§RVK§§:',$sortkey)),'0',$sortkey)"/>
											<xsl:with-param name="teststring" select="'; '"/>
										</xsl:call-template>
									</SortOrder>
									<xsl:copy-of select="parent::RQItem/*"/>
								</Dokument>
							</xsl:when>
							<xsl:otherwise>
								<Dokument>
									<SortOrder>
										<xsl:for-each select="notation">
											<xsl:if test="starts-with(.,$class)">
												<xsl:value-of select="concat(.,'0',$sortkey)"/>
											</xsl:if>
										</xsl:for-each>
										<!--                    
										<xsl:value-of select="concat($class,'0',$sortkey)"/>
									-->
									</SortOrder>
									<!-- CODE 060615 -->
									<DocNo>
										<xsl:value-of select="@DocNo"/>
									</DocNo>
									<xsl:copy-of select="."/>
									<!-- CODE 060615 -->
									<Classification>
										<xsl:for-each select="notation">
											<xsl:value-of select="."/>
											<xsl:text>; </xsl:text>
										</xsl:for-each>
									</Classification>
								</Dokument>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:for-each>
				</xsl:variable>

				<xsl:for-each select="msxsl:node-set($DocList)/Dokument">
					<xsl:sort select="SortOrder"/>

					<xsl:copy-of select="."/>
				</xsl:for-each>
			</Systematik>
		</xsl:if>
	</xsl:template>


  <xsl:template name="getnoclassitems">
    <xsl:if
      test="count(//Dokument/ID[not(parent::*/Classification)]) + count(//Classification[string-length(.)=0 or starts-with(.,'§')]) + count(//folder[name(parent::node())!='folder' and name(parent::node())!='filelist'][not(notation)])+ count(//file[not(notation)][name(parent::node())!='folder' and name(parent::node())!='filelist'])">
      <Systematik>
        <Description>
          <xsl:text>Unclassified</xsl:text>
        </Description>
        <DDCNumber>
          <xsl:text>Z</xsl:text>
        </DDCNumber>
        <ClassPath>
          <xsl:text>Z Unclassified</xsl:text>
        </ClassPath>
        <Hits>
          <xsl:value-of
            select="count(//Dokument/ID[not(parent::*/Classification)]) + count(//Classification[string-length(.)=0 or starts-with(.,'§')]) + count(//folder[name(parent::node())!='folder' and name(parent::node())!='filelist'][not(notation)]) + count(//file[not(notation)][name(parent::node())!='folder' and name(parent::node())!='filelist'])"
          />
        </Hits>
        <xsl:for-each
          select="//Dokument/ID[not(parent::*/Classification)] | //Classification[string-length(.)=0 or starts-with(.,'§')] | //folder[name(parent::node())!='folder' and name(parent::node())!='filelist'][not(notation)] | //file[not(notation)][name(parent::node())!='folder' and name(parent::node())!='filelist']">
          <xsl:sort select="parent::*/Feld30 | @Feld30"/>
          <xsl:choose>
            <xsl:when test="(name(.)='ID')">
              <Dokument>
                <SortOrder>
                  <xsl:value-of select="parent::*/Feld30"/>
                </SortOrder>
                <xsl:copy-of select="parent::RQItem/*"/>
                <Classification>
                  <xsl:text></xsl:text>
                </Classification>
              </Dokument>
            </xsl:when>
            <xsl:when test="(name(.)='Classification')">
              <Dokument>
                <SortOrder>
                  <xsl:value-of select="parent::*/Feld30"/>
                </SortOrder>
                <xsl:copy-of select="parent::RQItem/*"/>
              </Dokument>
            </xsl:when>
            <xsl:otherwise>
              <Dokument>
                <SortOrder>
                  <xsl:value-of select="@Feld30"/>
                </SortOrder>
                <!-- CODE 060615 -->
                <DocNo>XXX</DocNo>
                <xsl:copy-of select="."/>
                <!-- CODE 060615 -->
                <Classification>
                  <xsl:for-each select="notation">
                    <xsl:value-of select="."/>
                    <xsl:text>;</xsl:text>
                  </xsl:for-each>
                </Classification>
              </Dokument>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>
      </Systematik>
    </xsl:if>
  </xsl:template>


  <xsl:template name="write-document">
    <xsl:variable name="ListNr">
      <xsl:choose>
        <xsl:when test="/Systematik/Dokument">
          <xsl:number level="any" count="/Systematik/Dokument" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:number level="any" count="//Dokument" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:variable name="EID">
      <xsl:value-of select="generate-id()"/>
    </xsl:variable>

    <li>
      <xsl:if test="name(preceding-sibling::*[1])='Hits' and string-length(parent::*/DDCNumber)>0">
        <span class="category" pos="position()">
          <xsl:attribute name="class">
            <xsl:text>category</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="pos">
            <xsl:value-of select="position() - 1"/>
          </xsl:attribute>
          <A>
            <xsl:attribute name="name">
              <xsl:value-of select="parent::*/DDCNumber" />
            </xsl:attribute>
            <xsl:value-of select="parent::*/DDCNumber" />
          </A>
          <xsl:text> - </xsl:text>
          <xsl:value-of select="parent::*/Description" />
        </span>
      </xsl:if>
      <table class="itemlist">
        <tr>
          <td class="rq-fieldname" align="left" valign="top" width="30px">
            <xsl:value-of select="$ListNr" />
          </td>
          <td align="center" valign="top" width="35px">
            <xsl:choose>
              <xsl:when test="contains(DocTypeName,'Monographie')">
                <img title="Monograph">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_books.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Broschüre')">
                <img title="Broschure">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_books.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Report')">
                <img title="Report">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_books.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Dissertation')">
                <img title="Thesis">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_books.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Manuskript')">
                <img title="Manuscript">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_hand.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Werk')">
                <img title="Werk">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_hand.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Tonaufzeichnung')">
                <img title="Audio">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_sound.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Zeitschrift')">
                <img title="Journal">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_per.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Aufsatz')">
                <img title="Essay">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_art.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Artikel')">
                <img title="Article">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_article.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Beitrag')">
                <img title="Contribution">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_contr.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Bildaufzeichnung')">
                <img title="Film/Video">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_avm.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Software')">
                <img title="Software">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_software.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Daten')">
                <img title="Data">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_software.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Datenbank')">
                <img title="Software">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_software.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Digitale Dokumentsammlung')">
                <img title="Software">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_software.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Multimedia')">
                <img title="Multimedia">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_mmedia.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Konvolut')">
                <img title="Document Collection">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_docs.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Karte')">
                <img title="Map">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_map.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Notendruck')">
                <img title="Musical Print">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_music.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Bildpräsentation')">
                <img title="Picture">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_pictures.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Schriftenreihe')">
                <img title="Series">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_series.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Sammlung')">
                <img title="Collected Works">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_books.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Sammelwerk')">
                <img title="Collection">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_books.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName,'Mehrbandwerk')">
                <img title="Multi Volume">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_series.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName, 'folder')">
                <img title="Web Source Collection">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_webfolder.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:when test="contains(DocTypeName, 'file')">
                <img title="Web Source">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_websource.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:when>
              <xsl:otherwise>
                <img title="">
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ApplPath,'/Images/icon_art.gif')" />
                  </xsl:attribute>
                </img>
              </xsl:otherwise>
            </xsl:choose>
          </td>
          <td align="left" valign="top">
            <xsl:element name="a">
              <xsl:attribute name="href">
                <xsl:text>javascript:getRQItem(&quot;</xsl:text>
                <xsl:value-of select="DocNo"/>
                <xsl:text>&quot;, &quot;</xsl:text>
                <xsl:value-of select="$EID"/>
                <xsl:text>&quot;, &quot;</xsl:text>
                <xsl:value-of select="concat('o',$EID)"/>
                <xsl:text>&quot;);</xsl:text>
              </xsl:attribute>
              <xsl:apply-templates select="file/@name | folder/@name | Title" mode="list"/>
            </xsl:element>
            <xsl:apply-templates select="file/@comment | folder/@comment | Abstract" mode="list" >
              <xsl:with-param name="XID" select="concat('o',$EID)" />
            </xsl:apply-templates>
            <xsl:element name="div">
              <xsl:attribute name="DocNo">
                <xsl:value-of select="DocNo"/>
              </xsl:attribute>
              <xsl:attribute name="id">
                <xsl:value-of select="$EID"/>
              </xsl:attribute>
              <xsl:attribute name="style">
                <xsl:text>display:none</xsl:text>
              </xsl:attribute>
            </xsl:element>
          </td>
          <!--<td>
            <xsl:value-of select="SortOrder"/>
            <xsl:value-of select="Feld30"/>
          </td>-->
        </tr>
      </table>
    </li>
  </xsl:template>


  <xsl:template match="Title" mode="list">
    <xsl:variable name="strTitle" select="." />

    <xsl:choose>
      <xsl:when test="contains(.,'|')">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "normalize-space(substring-before(.,'|'))" />
        </xsl:call-template>
      </xsl:when>
      <xsl:when test="contains(.,'=')">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "normalize-space(substring-before(.,'='))" />
        </xsl:call-template>
      </xsl:when>
      <xsl:when test="contains(.,':')">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "normalize-space(substring-before(.,':'))" />
        </xsl:call-template>
      </xsl:when>
      <xsl:when test="contains(.,'/')">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "normalize-space(substring-before(.,'/'))" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "." />
        </xsl:call-template>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
  
  <xsl:template match="Authors" mode="list">
    <xsl:choose>
      <xsl:when test="(string(.) !='') and (string(.) != ' ')">
        <tr>
          <td>
            <xsl:choose>
              <xsl:when test="string-length(.) &lt;= 40">
                <xsl:call-template name = "escape-apos">
                  <xsl:with-param name = "string" select = "." />
                </xsl:call-template>
              </xsl:when>
              <xsl:when test="contains(substring(.,30),';')">
                <xsl:call-template name = "escape-apos">
                  <xsl:with-param name = "string" select = "normalize-space(concat(substring(.,1,39),substring-before(substring(.,40),';'),'; ...'))" />
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
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates select="../Institutions" mode="list" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <xsl:template match="Institutions" mode="list">
    <xsl:if test="(string(.) !='') and (string(.) != ' ')">
      <tr>
        <td>
          <xsl:choose>
            <xsl:when test="contains(.,';')">
              <xsl:call-template name = "escape-apos">
                <xsl:with-param name = "string" select = "normalize-space(substring-before(.,';'))" />
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
    </xsl:if>
  </xsl:template>


  <xsl:template match="Locality" mode="list">
    <xsl:variable name="et">
      <xsl:value-of select="concat(.,../Publisher,../PublTime)"/>
    </xsl:variable>
    
    <xsl:if test="$et !='' and $et != ' '">
      <tr>
        <td>
          <xsl:if test="(string(.) !='') and (string(.) != ' ')">
            <xsl:choose>
              <xsl:when test="substring(.,string-length(.)) = ';'">
                <xsl:call-template name = "escape-apos">
                  <xsl:with-param name = "string" select = "normalize-space(substring(.,1,string-length(.)-1))" />
                </xsl:call-template>
              </xsl:when>
              <xsl:otherwise>
                <xsl:call-template name = "escape-apos">
                  <xsl:with-param name = "string" select = "." />
                </xsl:call-template>
              </xsl:otherwise>
            </xsl:choose>
            <xsl:choose>
              <xsl:when test="(string(../Publisher) !='') and (string(../Publisher) != ' ')">
                <xsl:text> : </xsl:text>
              </xsl:when>
              <xsl:when test="(string(../PublTime) !='') and (string(../PublTime) != ' ')">
                <xsl:text>; </xsl:text>
              </xsl:when>
            </xsl:choose>
          </xsl:if>

          <xsl:if test="(string(../Publisher) !='') and (string(../Publisher) != ' ')">
            <xsl:choose>
              <xsl:when test="substring(../Publisher,string-length(../Publisher)) = ';'">
                <xsl:call-template name = "escape-apos">
                  <xsl:with-param name = "string" select = "normalize-space(substring(../Publisher,1,string-length(../Publisher)-1))" />
                </xsl:call-template>
              </xsl:when>
              <xsl:otherwise>
                <xsl:call-template name = "escape-apos">
                  <xsl:with-param name = "string" select = "../Publisher" />
                </xsl:call-template>
              </xsl:otherwise>
            </xsl:choose>
            <xsl:if test="(string(../PublTime) !='') and (string(../PublTime) != ' ')">
              <xsl:text> ; </xsl:text>
            </xsl:if>
          </xsl:if>

          <xsl:if test="(string(../PublTime) !='') and (string(../PublTime) != ' ')">
            <xsl:choose>
              <xsl:when test="substring(../PublTime,string-length(../PublTime)) = ';'">
                <xsl:call-template name = "escape-apos">
                  <xsl:with-param name = "string" select = "normalize-space(substring(../PublTime,1,string-length(../PublTime)-1))" />
                </xsl:call-template>
              </xsl:when>
              <xsl:otherwise>
                <xsl:call-template name = "escape-apos">
                  <xsl:with-param name = "string" select = "../PublTime" />
                </xsl:call-template>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:if>
        </td>
      </tr>
    </xsl:if>
  </xsl:template>


  <xsl:template match="Title" mode="list">
    <xsl:variable name="strTitle" select="." />

    <xsl:choose>
      <xsl:when test="contains(.,'|')">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "normalize-space(substring-before(.,'|'))" />
        </xsl:call-template>
      </xsl:when>
      <xsl:when test="contains(.,'=')">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "normalize-space(substring-before(.,'='))" />
        </xsl:call-template>
      </xsl:when>
      <xsl:when test="contains(.,':')">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "normalize-space(substring-before(.,':'))" />
        </xsl:call-template>
      </xsl:when>
      <xsl:when test="contains(.,'/')">
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "normalize-space(substring-before(.,'/'))" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name = "escape-apos">
          <xsl:with-param name = "string" select = "." />
        </xsl:call-template>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  
  <xsl:template match="Signature[(string(.) !='') and (string(.) != ' ')]" mode="list">
    <xsl:variable name="IdNr" select="../ID" />
    <xsl:variable name="DocNo" select="../DocNo" />
    <xsl:variable name="Signatures">
      <xsl:call-template name="ParseSubFields">
        <xsl:with-param name="FieldSubString" select="concat(.,';')" />
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="ShortTitle">
      <xsl:call-template name="generate-st" />
    </xsl:variable>

    <xsl:for-each select="msxsl:node-set($Signatures)/SubFields">
      <xsl:choose>
        <xsl:when test="contains(.,'MyDoc=')">
          <a>
            <xsl:attribute name="href">
              <xsl:call-template name = "escape-apos">
<!--                
                <xsl:with-param name = "string" select = "concat($MyDocsPath,'/',substring-after(.,'MyDoc='),'&amp;TI=', $ShortTitle)" />
-->                
                <xsl:with-param name = "string" select = "concat($ApplPath,'/ItemViewer/', $DocNo, '?itemAdress=MyDocs/', substring-after(.,'MyDoc='))" />
              </xsl:call-template>
            </xsl:attribute>
            <xsl:attribute name="target">
              <xsl:text>_new</xsl:text>
            </xsl:attribute>
            <img title="View Full Text">
              <xsl:attribute name="src">
                <xsl:value-of select="concat($ApplPath,'/Images/icon_fulltext.gif')" />
              </xsl:attribute>
              <xsl:attribute name="align">absmiddle</xsl:attribute>
              <xsl:attribute name="border">0</xsl:attribute>
              <xsl:attribute name="hspace">5</xsl:attribute>
            </img>
          </a>
        </xsl:when>
        <xsl:when test="contains(.,'MyVideo=')">
          <a>
            <xsl:attribute name="href">
              <xsl:call-template name = "escape-apos">
                <xsl:with-param name = "string" select = "concat($ApplPath,'/itemViewer/', $DocNo, '?itemAdress=MyVideo/', substring-after(.,'MyVideo='))" />
              </xsl:call-template>
            </xsl:attribute>
            <xsl:attribute name="target">
              <xsl:text>_new</xsl:text>
            </xsl:attribute>
            <img title="View Online Video">
              <xsl:attribute name="src">
                <xsl:value-of select="concat($ApplPath,'/Images/icon_listenaudio.gif')" />
              </xsl:attribute>
              <xsl:attribute name="align">absmiddle</xsl:attribute>
              <xsl:attribute name="border">0</xsl:attribute>
              <xsl:attribute name="hspace">5</xsl:attribute>
            </img>
          </a>
        </xsl:when>
        <xsl:when test="contains(.,'MyMusic=')">
          <a>
            <xsl:attribute name="href">
              <xsl:call-template name = "escape-apos">
                <xsl:with-param name = "string" select = "concat($ApplPath,'/ItemViewer/', $DocNo, '?itemAdress=MyMusic/', substring-after(.,'MyMusic='))" />
              </xsl:call-template>
            </xsl:attribute>
            <xsl:attribute name="target">
              <xsl:text>_new</xsl:text>
            </xsl:attribute>
            <img title="Listen Online Audio">
              <xsl:attribute name="src">
                <xsl:value-of select="concat($ApplPath,'/Images/icon_listenaudio.gif')" />
              </xsl:attribute>
              <xsl:attribute name="align">absmiddle</xsl:attribute>
              <xsl:attribute name="border">0</xsl:attribute>
              <xsl:attribute name="hspace">5</xsl:attribute>
            </img>
          </a>
        </xsl:when>
        <xsl:when test="contains(.,'http://')">
          <a>
            <xsl:attribute name="href">
              <xsl:call-template name = "escape-apos">
                <xsl:with-param name = "string" select = "concat('http://',substring-after(.,'http://'))" />
              </xsl:call-template>
            </xsl:attribute>
            <xsl:attribute name="target">
              <xsl:text>_new</xsl:text>
            </xsl:attribute>
            <img>
              <xsl:attribute name="title">
                <xsl:call-template name = "escape-apos">
                  <xsl:with-param name = "string" select = "concat('External Link: ','http://',substring-after(.,'http://'))" />
                </xsl:call-template>
              </xsl:attribute>
              <xsl:attribute name="src">
                <xsl:value-of select="concat($ApplPath,'/Images/icon_fulltext.gif')" />
              </xsl:attribute>
              <xsl:attribute name="align">absmiddle</xsl:attribute>
              <xsl:attribute name="border">0</xsl:attribute>
              <xsl:attribute name="hspace">5</xsl:attribute>
            </img>
          </a>
        </xsl:when>
        <xsl:when test="contains(.,'rtsp://')">
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
            <img title="RTSP Link">
              <xsl:attribute name="src">
                <xsl:value-of select="concat($ApplPath,'/Images/icon_fulltext.gif')" />
              </xsl:attribute>
              <xsl:attribute name="align">absmiddle</xsl:attribute>
              <xsl:attribute name="border">0</xsl:attribute>
              <xsl:attribute name="hspace">5</xsl:attribute>
            </img>
          </a>
        </xsl:when>
        <xsl:otherwise>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:for-each>
  </xsl:template>


  <xsl:template match="DocTypeName" mode="list">
    <xsl:choose>
      <xsl:when test="contains(.,'folder')">
        <tr>
          <td>
            <xsl:call-template name = "escape-apos">
              <xsl:with-param name = "string" select = "concat('created ',substring(../CreateTime,7,2),'.',substring(../CreateTime,5,2),'.',substring(../CreateTime,1,4)) " />
            </xsl:call-template>
          </td>
        </tr>
        <tr>
          <td>
            <xsl:call-template name = "escape-apos">
              <xsl:with-param name = "string" select = "'web source collection'" />
            </xsl:call-template>
          </td>
        </tr>
      </xsl:when>
      <xsl:when test="contains(.,'file')">
        <tr>
          <td>
            <xsl:call-template name = "escape-apos">
              <xsl:with-param name = "string" select = "concat('bookmarked ',substring(../CreateTime,7,2),'.',substring(../CreateTime,5,2),'.',substring(../CreateTime,1,4)) " />
            </xsl:call-template>
          </td>
        </tr>
        <tr>
          <td>
            <a class="comment">
              <xsl:attribute name="href">
                <xsl:call-template name = "escape-apos">
                  <xsl:with-param name = "string" select = "../Signature" />
                </xsl:call-template>
              </xsl:attribute>
              <xsl:attribute name="target">
                <xsl:value-of select="&quot;_NEW&quot;" />
              </xsl:attribute>
              <xsl:choose>
                <xsl:when test="string-length(../Signature) &gt; 60">
                  <xsl:call-template name = "escape-apos">
                    <xsl:with-param name = "string" select = "concat(substring(../Signature,1,60),'...')" />
                  </xsl:call-template>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:call-template name = "escape-apos">
                    <xsl:with-param name = "string" select = "../Signature" />
                  </xsl:call-template>
                </xsl:otherwise>
              </xsl:choose>
            </a>
          </td>
        </tr>
      </xsl:when>
    </xsl:choose>
  </xsl:template>


  <xsl:template match="@comment | Abstract" mode="list">
    <xsl:param name="XID" select="''" />
    
    <xsl:variable name="EID">
      <xsl:value-of select="generate-id()"/>
    </xsl:variable>

    <xsl:variable name="abstr">
      <xsl:choose>
        <xsl:when test="substring-after(.,'$$TOC$$=')">
          <xsl:value-of select = "substring-before(.,'$$TOC$$=')" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select = "." />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:if test="string-length($abstr) > 0">
      <xsl:element name="a">
        <xsl:attribute name="href">
          <xsl:text>javascript:visibilityToggle('</xsl:text>
          <xsl:value-of select="$EID" />
          <xsl:text>')</xsl:text>
        </xsl:attribute>
        <img title="Abstract">
          <xsl:attribute name="src">
            <xsl:value-of select="concat($ApplPath,'/Images/icon_abstract.gif')" />
          </xsl:attribute>
          <xsl:attribute name="align">absmiddle</xsl:attribute>
          <xsl:attribute name="border">0</xsl:attribute>
          <xsl:attribute name="hspace">5</xsl:attribute>
        </img>
      </xsl:element>
    </xsl:if>
    <xsl:apply-templates select="../Signature" mode="list"/>
    <xsl:element name="span">
      <xsl:attribute name="ID">
        <xsl:value-of select="$XID" />
      </xsl:attribute>
      <table border="0" class="comment" cellspacing="0" cellpadding="0">
        <xsl:apply-templates select="../Authors" mode="list" />
        <xsl:apply-templates select="../Locality" mode="list"/>
        <xsl:apply-templates select="../DocTypeName" mode="list"/>
        <xsl:apply-templates select="../@type" mode="list"/>
      </table>
    </xsl:element>
    <xsl:if test="string-length($abstr) > 0">
      <xsl:element name="div">
        <xsl:attribute name="ID">
          <xsl:value-of select="$EID" />
        </xsl:attribute>
        <xsl:attribute name="style">
          <xsl:text>display : none;</xsl:text>
        </xsl:attribute>
        <table border="0" cellspacing="0" cellpadding="10">
          <tr>
            <td class="comment">
              <xsl:call-template name = "escape-apos">
                <xsl:with-param name = "string" select = "$abstr" />
              </xsl:call-template>
            </td>
          </tr>
        </table>
      </xsl:element>
    </xsl:if>
  </xsl:template>


  <xsl:template name="escape-apos">
    <xsl:param name="string" />

    <xsl:value-of select="$string" disable-output-escaping="no"/>
  </xsl:template>


  <xsl:template match="/">
    <ul class="content">
      <xsl:choose>
        <xsl:when test="contains($SortType, 'Fach')">
          <xsl:apply-templates select="//Systematiken"/>
<!--
        <xsl:apply-templates select="/QueryResults/extDBList"/>
-->
        </xsl:when>
        <xsl:when test="contains($SortType, 'Regal')">
          <xsl:call-template name="get-shelves" />          
        </xsl:when>        
      </xsl:choose>
    </ul>
  </xsl:template>


  <xsl:template name="get-shelves">
    <xsl:variable name="ShelfList">
      <xsl:variable name="docs" select="/RQResultList/RQResultSet/RQItem" />

      <xsl:variable name="shelvesDesc" select="document('../../xml/shelves.xml')" />

      <xsl:call-template name="getitems-by-shelf">
        <xsl:with-param name="DocNodeSet" select="/RQResultList/RQResultSet/RQItem[contains(Signature, 'BU=R01')]" />
        <xsl:with-param name="ShelfType" select="'Hauptregal'" />
      </xsl:call-template>
      <xsl:call-template name="getitems-by-shelf">
        <xsl:with-param name="DocNodeSet" select="/RQResultList/RQResultSet/RQItem[contains(Signature, 'BU=R02')]" />
        <xsl:with-param name="ShelfType" select="'Biographien'" />
      </xsl:call-template>
      <xsl:call-template name="getitems-by-shelf">
        <xsl:with-param name="DocNodeSet" select="/RQResultList/RQResultSet/RQItem[contains(Signature, 'BU=R03')]" />
        <xsl:with-param name="ShelfType" select="'Belletristik'" />
      </xsl:call-template>
      <xsl:call-template name="getitems-by-shelf">
        <xsl:with-param name="DocNodeSet" select="/RQResultList/RQResultSet/RQItem[contains(Signature, 'BU=') and not(contains(Signature, 'BU=R01')) and not(contains(Signature, 'BU=R02')) and not(contains(Signature, 'BU=R03'))]" />
        <xsl:with-param name="ShelfType" select="'Unbearbeitet'" />
      </xsl:call-template>
      <xsl:for-each select="$shelvesDesc//tag">
        <xsl:variable name="ShelfID" select="concat(string(@name), @delimiter)" />

        <xsl:if test="count($docs[contains(Signature, $ShelfID)]) > 0">
          <Systematik>
            <Description>
              <xsl:value-of select="@text" />
            </Description>
            <DDCNumber>
              <xsl:value-of select="@name" />
            </DDCNumber>
            <ClassPath>
              <xsl:value-of select="concat(@name, ' (', @text, ') ')" />
            </ClassPath>
            <Hits>
              <xsl:value-of select="count($docs[contains(Signature, $ShelfID)])" />
            </Hits>
            <xsl:for-each select="$docs[contains(Signature, $ShelfID)]">
              <xsl:sort select="substring-before(substring-after(concat(Signature, ';'), $ShelfID), ';')" />
              <Dokument>
                <SortOrder>
                  <xsl:value-of select="substring-before(substring-after(concat(Signature, ';'), $ShelfID), ';')"/>
                </SortOrder>
                <xsl:copy-of select="./*"/>
              </Dokument>
            </xsl:for-each>
          </Systematik>
        </xsl:if>
      </xsl:for-each>
    </xsl:variable>

    <xsl:for-each select="msxsl:node-set($ShelfList)/Systematik/Dokument">
      <xsl:call-template name="write-document" />
    </xsl:for-each>  
  </xsl:template>
	
  
  <xsl:template match="Systematiken">
    <xsl:variable name="CatList">
      <xsl:for-each select="Systematik">
        <xsl:sort select="DDCNumber"/>
        <xsl:variable name="class" select="DDCNumber"/>
        <xsl:variable name="PID" select="ID"/>
        <xsl:call-template name="getitems-by-class">
          <xsl:with-param name="class" select="$class"/>
          <xsl:with-param name="DocNodeSet"	select="//Classification[starts-with(.,$class) or (contains(.,concat(' ',string($class))))]"/>
          <xsl:with-param name="DirNodeSet"	select="//notation[(substring(.,1,string-length($class))=$class) and (not(substring(preceding-sibling::notation,1,string-length($class))=$class))]"	/>
        </xsl:call-template>
      </xsl:for-each>
      <xsl:call-template name="getnoclassitems"/>
    </xsl:variable>

    <xsl:for-each select="msxsl:node-set($CatList)/Systematik/Dokument">
      <xsl:call-template name="write-document" />
<!--
      <xsl:variable name="ListNr">
        <xsl:choose>
          <xsl:when test="/Systematik/Dokument">
            <xsl:number level="any" count="/Systematik/Dokument" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:number level="any" count="//Dokument" />
          </xsl:otherwise>
        </xsl:choose>
      </xsl:variable>

      <xsl:variable name="EID">
        <xsl:value-of select="generate-id()"/>
      </xsl:variable>

      <li>
        <xsl:if test="name(preceding-sibling::*[1])='Hits' and string-length(parent::*/DDCNumber)>0">
          <span class="category" pos="position()">
            <xsl:attribute name="class">
              <xsl:text>category</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="pos">
              <xsl:value-of select="position() - 1"/>
            </xsl:attribute>
            <A>
              <xsl:attribute name="name">
                <xsl:value-of select="parent::*/DDCNumber" />
              </xsl:attribute>
              <xsl:value-of select="parent::*/DDCNumber" />
            </A>
            <xsl:text> - </xsl:text>
            <xsl:value-of select="parent::*/Description" />
          </span>
        </xsl:if>
        <table class="itemlist">
          <tr>
            <td class="rq-fieldname" align="left" valign="top" width="30px">
              <xsl:value-of select="$ListNr" />
            </td>
            <td align="center" valign="top" width="35px">
              <xsl:choose>
                <xsl:when test="contains(DocTypeName,'Monographie')">
                  <img title="Monograph">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_books.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Broschüre')">
                  <img title="Broschure">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_books.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Report')">
                  <img title="Report">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_books.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Dissertation')">
                  <img title="Thesis">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_books.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Manuskript')">
                  <img title="Manuscript">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_hand.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Werk')">
                  <img title="Werk">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_hand.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Tonaufzeichnung')">
                  <img title="Audio">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_sound.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Zeitschrift')">
                  <img title="Journal">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_per.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Aufsatz')">
                  <img title="Essay">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_art.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Artikel')">
                  <img title="Article">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_article.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Beitrag')">
                  <img title="Contribution">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_contr.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Bildaufzeichnung')">
                  <img title="Film/Video">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_avm.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Software')">
                  <img title="Software">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_software.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Daten')">
                  <img title="Data">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_software.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Datenbank')">
                  <img title="Software">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_software.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Digitale Dokumentsammlung')">
                  <img title="Software">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_software.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Multimedia')">
                  <img title="Multimedia">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_mmedia.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Konvolut')">
                  <img title="Document Collection">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_docs.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Karte')">
                  <img title="Map">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_map.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Notendruck')">
                  <img title="Musical Print">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_music.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Bildpräsentation')">
                  <img title="Picture">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_pictures.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Schriftenreihe')">
                  <img title="Series">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_series.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Sammlung')">
                  <img title="Collected Works">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_books.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Sammelwerk')">
                  <img title="Collection">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_books.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName,'Mehrbandwerk')">
                  <img title="Multi Volume">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_series.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName, 'folder')">
                  <img title="Web Source Collection">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_webfolder.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:when test="contains(DocTypeName, 'file')">
                  <img title="Web Source">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_websource.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:when>
                <xsl:otherwise>
                  <img title="">
                    <xsl:attribute name="src">
                      <xsl:value-of select="concat($ApplPath,'/Images/icon_art.gif')" />
                    </xsl:attribute>
                  </img>
                </xsl:otherwise>
              </xsl:choose>
            </td>
            <td align="left" valign="top">
              <xsl:element name="a">
                <xsl:attribute name="href">
                  <xsl:text>javascript:getRQItem(&quot;</xsl:text>
                  <xsl:value-of select="DocNo"/>
                  <xsl:text>&quot;, &quot;</xsl:text>
                  <xsl:value-of select="$EID"/>
                  <xsl:text>&quot;, &quot;</xsl:text>
                  <xsl:value-of select="concat('o',$EID)"/>
                  <xsl:text>&quot;);</xsl:text>
                </xsl:attribute>
                <xsl:apply-templates select="file/@name | folder/@name | Title" mode="list"/>
              </xsl:element>
              <xsl:apply-templates select="file/@comment | folder/@comment | Abstract" mode="list" >
                <xsl:with-param name="XID" select="concat('o',$EID)" />
              </xsl:apply-templates>
              <xsl:element name="div">
                <xsl:attribute name="DocNo">
                  <xsl:value-of select="DocNo"/>
                </xsl:attribute>
                <xsl:attribute name="id">
                  <xsl:value-of select="$EID"/>
                </xsl:attribute>
                <xsl:attribute name="style">
                  <xsl:text>display:none</xsl:text>
                </xsl:attribute>
              </xsl:element>
            </td>
-->            
            <!--              
              <td>
                <xsl:value-of select="Feld30"/>
              </td>
-->
<!--      
          </tr>
        </table>
      </li>
-->      
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>