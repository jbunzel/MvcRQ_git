<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
  xmlns:mo="http://www.loc.gov/mods/v3">
<!--
	Transforms raw data delivered from NewMetaQuery.DoMetaSearch into RQResult Format
	Sort Order: by shelf
  Modification a: All signatures not only the first are evaluated.
	-->

<!--
	<xsl:include href="RQTransExtDB.xslt" />
-->

	<xsl:param name="classname" select="''"/>

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
		<!-- 
       selects the first substring of the semicolon seperated list of strings $list, 
       if that starts with any of the substrings contained in the semicolon seperated 
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
					<xsl:when test="substring($wlist,1,string-length(concat(';',$first)))=concat(';',$first)">
						<xsl:variable name="v1"
              select="concat($first,substring-after($wlist,concat(';',$first)))"/>

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


	<xsl:template name="SortItemsByClass">
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
						<xsl:sort select="PublTime" order="descending"/>

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

	
	<xsl:template name="GetShelves">

		<xsl:call-template name="SortItemsByClass">
			<xsl:with-param name="DocNodeSet" select="/RQResultList/RQResultSet/RQItem[contains(Signature, 'BU=R01')]" />
			<xsl:with-param name="ShelfType" select="'Hauptregal'" />
		</xsl:call-template>

		<xsl:call-template name="SortItemsByClass">
			<xsl:with-param name="DocNodeSet" select="/RQResultList/RQResultSet/RQItem[contains(Signature, 'BU=R02')]" />
			<xsl:with-param name="ShelfType" select="'Biographien'" />
		</xsl:call-template>

		<xsl:call-template name="SortItemsByClass">
			<xsl:with-param name="DocNodeSet" select="/RQResultList/RQResultSet/RQItem[contains(Signature, 'BU=R03')]" />
			<xsl:with-param name="ShelfType" select="'Belletristik'" />
		</xsl:call-template>

		<xsl:call-template name="SortItemsByClass">
			<xsl:with-param name="DocNodeSet" select="/RQResultList/RQResultSet/RQItem[contains(Signature, 'BU=') and not(contains(Signature, 'BU=R01')) and not(contains(Signature, 'BU=R02')) and not(contains(Signature, 'BU=R03'))]" />
			<xsl:with-param name="ShelfType" select="'Unbearbeitet'" />
		</xsl:call-template>

    <xsl:variable name="docs" select="/RQResultList/RQResultSet/RQItem" />

    <xsl:variable name="shelvesDesc" select="document('../../xml/shelves.xml')" />
    
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
	</xsl:template>


	<xsl:template match="/">
    <ul class="content">
      <xsl:call-template name="GetShelves" />
		</ul>
	</xsl:template>

</xsl:stylesheet>
