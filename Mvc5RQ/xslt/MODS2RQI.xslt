<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
	xmlns:zs="http://www.loc.gov/zing/srw/"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:mo="http://www.loc.gov/mods/v3" 
	xmlns:rq="http://www.riquest.de/rq/">


	<xsl:template name="get-number">
		<xsl:param name="numberstring"/>

		<xsl:choose>
			<xsl:when test="string-length($numberstring)>0">
				<xsl:choose>
					<xsl:when test="string(number($numberstring))='NaN'">
						<xsl:choose>
							<xsl:when test="contains('1234567890',substring($numberstring,1,1))">
								<xsl:call-template name="get-number">
									<xsl:with-param name="numberstring" select="substring($numberstring,1,string-length($numberstring)-1)"/>
								</xsl:call-template>
							</xsl:when>
							<xsl:when test="contains('1234567890',substring($numberstring,string-length($numberstring),1))">
								<xsl:call-template name="get-number">
									<xsl:with-param name="numberstring" select="substring($numberstring,2,string-length($numberstring)-1)"/>
								</xsl:call-template>
							</xsl:when>
							<xsl:otherwise>
								<xsl:call-template name="get-number">
									<xsl:with-param name="numberstring" select="substring($numberstring,2,string-length($numberstring)-2)"/>
								</xsl:call-template>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="number($numberstring)" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="'0000'" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>


	<xsl:template name="parseMODSdoc">
		<xsl:param name="sortType" select="'byDate'" />
		
		<SortOrder>
			<xsl:choose>
				<xsl:when test="contains($sortType,'byTitle')">
					<xsl:choose>
						<xsl:when test="mo:titleInfo/mo:nonSort">
							<xsl:value-of select="concat(mo:titleInfo/mo:nonSort,' ', mo:titleInfo/mo:title)"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="mo:titleInfo/mo:title"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="get-number">
						<xsl:with-param name="numberstring" select="mo:originInfo/mo:dateIssued"/>
					</xsl:call-template>
				</xsl:otherwise>
			</xsl:choose>
		</SortOrder>
		<ID></ID>
		<DocNo></DocNo>
		<xsl:element name="Title">
			<xsl:if test="mo:titleInfo/mo:nonSort">
				<xsl:value-of select="concat(mo:titleInfo/mo:nonSort,' ')"/>
			</xsl:if>
			<xsl:value-of select="mo:titleInfo/mo:title"/>
      <xsl:if test="not(mo:titleInfo/mo:title)">
        <xsl:for-each select="rq:toc">
          <xsl:variable name="titleStr">
            <xsl:value-of select="rq:tocTitle" />
            <xsl:if test="string-length(rq:tocStatement)>0">
              <xsl:value-of select="concat(' / ',rq:tocStatement)" />
            </xsl:if>
          </xsl:variable>
          <xsl:choose>
            <xsl:when test="(last()>1) and (substring($titleStr,string-length($titleStr),1)='.')">
              <xsl:value-of select="concat(substring($titleStr,1,string-length($titleStr)-1),'. - ')" /> 
            </xsl:when>
            <xsl:when test="last()>1">
              <xsl:value-of select="concat($titleStr,'. - ')" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="$titleStr" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>
      </xsl:if>
			<xsl:if test="mo:titleInfo/mo:subTitle">
				<xsl:value-of select="concat(' : ',mo:titleInfo/mo:subTitle)"/>
			</xsl:if>
			<xsl:if test="mo:note[@type='statement of responsibility']">
				<xsl:value-of select="concat(' / ', mo:note[@type='statement of responsibility'])"/>
			</xsl:if>
		</xsl:element>
		<xsl:element name="Authors">
			<xsl:for-each select="mo:name[@type='personal']">
          <xsl:choose>
            <xsl:when test="mo:namePart[@type='family']">
              <xsl:value-of select="mo:namePart[@type='family']" />
              <xsl:if test="mo:namePart[@type='given']">
                <xsl:value-of select="concat(', ', mo:namePart[@type='given'])" />
              </xsl:if>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="mo:namePart" />
            </xsl:otherwise>
          </xsl:choose>
				<xsl:if test="mo:role/mo:roleTerm[@type='text']">
          <xsl:if test="mo:role/mo:roleTerm != 'creator'">
            <xsl:value-of select="concat( ' {', mo:role/mo:roleTerm, '}')" />
          </xsl:if>
				</xsl:if>
        <xsl:if test="mo:role/mo:roleTerm[@type='code']">
          <xsl:choose>
            <xsl:when test="starts-with(mo:role/mo:roleTerm,'rsp')">
              <xsl:value-of select="' {Resp.}'" />
            </xsl:when>
          </xsl:choose>
        </xsl:if>
        <xsl:value-of select="'; '"/>
			</xsl:for-each>
		</xsl:element>
		<xsl:element name="Source">
			<xsl:choose>
				<xsl:when test="contains(mo:relatedItem[@type='host']/mo:titleInfo/mo:title,'in:')">
					<xsl:value-of select="normalize-space(substring-after(mo:relatedItem[@type='host']/mo:titleInfo/mo:title,'in:'))" />
				</xsl:when>
				<xsl:when test="contains(mo:relatedItem[@type='host']/mo:titleInfo/mo:title,'In:')">
					<xsl:value-of select="normalize-space(substring-after(mo:relatedItem[@type='host']/mo:titleInfo/mo:title,'In:'))" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="mo:relatedItem[@type='host']/mo:titleInfo/mo:title" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:element>
		<xsl:element name="Edition">
			<xsl:value-of select="mo:originInfo/mo:edition" />
		</xsl:element>
		<xsl:element name="ISDN">
			<xsl:for-each select="mo:identifier">
				<xsl:choose>
					<xsl:when test="@type='isbn'">
						<xsl:value-of select="concat('ISBN: ',.,'; ')"/>
					</xsl:when>
					<xsl:when test="@type='issn'">
						<xsl:value-of select="concat('ISSN: ',.,'; ')"/>
					</xsl:when>
				</xsl:choose>
			</xsl:for-each>
		</xsl:element>
		<Coden />
		<xsl:element name="Institutions">
			<xsl:for-each select="mo:name">
				<xsl:if test="mo:affiliation">
					<xsl:value-of select="concat(mo:affiliation, '{Affil.}; ')"/>
				</xsl:if>
			</xsl:for-each>
			<xsl:for-each select="mo:name[@type='corporate' or @type='conference']">
				<xsl:value-of select="concat(mo:namePart, '; ')"/>
			</xsl:for-each>
		</xsl:element>
		<xsl:element name="Series">
			<xsl:value-of select="mo:relatedItem[@type='series']/mo:titleInfo/mo:title" />
		</xsl:element>
		<xsl:element name="Locality">
			<xsl:for-each select="mo:originInfo/mo:place/mo:placeTerm[@type='text']">
				<xsl:value-of select="concat(., '; ')"/>
			</xsl:for-each>
		</xsl:element>
		<xsl:element name="Publisher">
			<xsl:for-each select="mo:originInfo/mo:publisher">
				<xsl:value-of select="concat(., '; ')"/>
			</xsl:for-each>
		</xsl:element>
		<xsl:element name="PublTime">
			<xsl:value-of select="mo:originInfo/mo:dateIssued" />
		</xsl:element>
		<xsl:element name="Volume">
			<!--							
							<xsl:value-of select="substring-before(mo:relatedItem[@type='host']/mo:part/mo:text,',')" />
-->
		</xsl:element>
		<xsl:element name="Issue">
			<!--							
							<xsl:value-of select="substring-after(substring-before(mo:relatedItem[@type='host']/mo:part/mo:text,', p.'),', ')" />
-->
		</xsl:element>
		<xsl:element name="Pages">
			<xsl:if test="mo:physicalDescription/mo:form">
				<xsl:value-of select="concat('[',mo:physicalDescription/mo:form, '] ')"/>
			</xsl:if>
			<xsl:if test="mo:relatedItem[@type='host']/mo:part/mo:text">
				<xsl:value-of select="concat(mo:relatedItem[@type='host']/mo:part/mo:text,'; ')" />
			</xsl:if>
			<xsl:value-of select="mo:physicalDescription/mo:extent" />
		</xsl:element>
		<xsl:element name="Language">
			<xsl:value-of select="mo:language/mo:languageTerm[@authority='iso639-2b' and @type='code']" />
		</xsl:element>
		<xsl:element name="Signature">
			<xsl:for-each select="(mo:location|mo:identifier)">
				<xsl:choose>
					<xsl:when test="mo:url">
						<xsl:value-of select="concat('https://proxy.nationallizenzen.de/login?url=',mo:url,'; ')"/>
					</xsl:when>
					<xsl:when test="@type='uri'">
						<xsl:value-of select="concat('extern: ',.,'; ')"/>
					</xsl:when>
				</xsl:choose>
			</xsl:for-each>
		</xsl:element>
		<DocTypeCode />
		<xsl:element name="DocTypeName">
			<xsl:choose>
				<xsl:when test="contains(mo:typeOfResource,'sound recording')">
					<xsl:value-of select="concat('Tonaufzeichnung', '; ')" />
				</xsl:when>
				<xsl:when test="contains(mo:typeOfResource,'moving image')">
					<xsl:value-of select="concat ('Bildaufzeichnung', '; ')" />
				</xsl:when>
				<xsl:when test="contains(mo:typeOfResource,'software')">
					<xsl:value-of select="concat('Software', '; ')" />
				</xsl:when>
				<xsl:when test="contains(mo:typeOfResource,'multimedia')">
					<xsl:value-of select="concat('Multimedia', '; ')" />
				</xsl:when>
				<xsl:when test="contains(mo:typeOfResource,'cartographic')">
					<xsl:value-of select="concat('Karte', '; ')" />
				</xsl:when>
				<xsl:when test="contains(mo:typeOfResource,'image')">
					<xsl:value-of select="concat('Bildpräsentation', '; ')" />
				</xsl:when>
				<xsl:when test="contains(mo:typeOfResource,'notaded music')">
					<xsl:value-of select="concat('Notendruck', '; ')" />
				</xsl:when>
				<xsl:when test="contains(mo:typeOfResource,'mixed material')">
					<xsl:value-of select="concat('Konvolut', '; ')" />
				</xsl:when>
				<xsl:when test="contains(mo:originInfo/mo:issuance,'monographic')">
					<xsl:value-of select="concat('Monographie', '; ')" />
				</xsl:when>
				<xsl:when test="contains(mo:originInfo/mo:issuance,'continuing')">
					<xsl:value-of select="concat('Schriftenreihe', '; ')" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="''"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:element>
		<xsl:element name="IndexTerms">
			<xsl:for-each select="mo:subject/mo:topic">
				<xsl:choose>
					<xsl:when test="starts-with(.,'[abr]')">
						<xsl:value-of select="concat(substring-after(.,'; '),'; ')"/>
						<xsl:value-of select="concat(substring-after(substring-before(.,';'),'[abr] '),'(=> {Abkz.}); ')"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="concat(.,'; ')"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:for-each>
			<xsl:for-each select="mo:subject/mo:occupation">
				<xsl:value-of select="concat(., '; ')"/>
			</xsl:for-each>
		</xsl:element>
		<xsl:element name="Subjects">
		</xsl:element>
		<xsl:element name="WorkType">
			<xsl:for-each select="mo:genre">
				<xsl:value-of select="concat(., '; ')"/>
			</xsl:for-each>
			<xsl:for-each select="mo:subject/mo:genre">
				<xsl:value-of select="concat(., '; ')"/>
			</xsl:for-each>
		</xsl:element>
		<xsl:element name="AboutLocation">
			<xsl:for-each select="mo:subject/mo:geographic">
				<xsl:value-of select="concat(., '; ')"/>
			</xsl:for-each>
		</xsl:element>
		<xsl:element name="AboutTime">
			<xsl:for-each select="mo:subject/mo:temporal">
				<xsl:value-of select="concat(., '; ')"/>
			</xsl:for-each>
		</xsl:element>
		<xsl:element name="AboutPersons">
			<xsl:for-each select="mo:subject/mo:name[@type='personal']">
        <xsl:choose>
          <xsl:when test="mo:namePart[@type='family']">
            <xsl:value-of select="mo:namePart[@type='family']" />
            <xsl:if test="mo:namePart[@type='given']">
              <xsl:value-of select="concat(', ', mo:namePart[@type='given'])" />
            </xsl:if>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="mo:namePart" />
          </xsl:otherwise>
        </xsl:choose>  
				<xsl:if test="mo:role/mo:roleTerm[@type='text']">
					<xsl:value-of select="concat( ' {', mo:role/mo:roleTerm, '}')" />
				</xsl:if>
				<xsl:value-of select="'; '"/>
			</xsl:for-each>
			<xsl:for-each select="mo:subject/mo:name[@type='corporate']">
				<xsl:value-of select="concat(mo:namePart, '; ')"/>
			</xsl:for-each>
		</xsl:element>
		<xsl:element name="Classification">
			<xsl:for-each select="mo:classification">
        <xsl:value-of select="concat('§§',translate(@authority,'abcdefghijklmnopqrstuvwxyzäöüß','ABCDEFGHIJKLMNOPQRSTUVWXYZ'),'§§:',translate(normalize-space(.),' ',''),'; ')"/>
			</xsl:for-each>
		</xsl:element>
		<xsl:element name="Abstract">
			<xsl:for-each select="mo:abstract">
				<xsl:if test="string-length(.)>50">
					<xsl:choose>
						<xsl:when test="starts-with(.,'abstract ')">
							<xsl:value-of select="concat(normalize-space(substring-after(.,'abstract ')),' ')" />
						</xsl:when>
						<xsl:when test="starts-with(.,'Abstract ')">
							<xsl:value-of select="concat(normalize-space(substring-after(.,'Abstract ')),' ')" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="concat(.,' ')" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>
			</xsl:for-each>
		</xsl:element>
		<CreateLocation></CreateLocation>
		<xsl:element name="CreateTime">
			<xsl:value-of select="mo:originInfo/mo:dateCreated" />
		</xsl:element>
		<Notes />
		<Feld30></Feld30>
	</xsl:template>
  

	<xsl:template name="getMODScollection">
		<xsl:if test="//zs:records">
			<Systematik>
<!--        
				<Description>
					<xsl:value-of select="../extDBList/@source"/>
				</Description>
				<DDCNumber>
					<xsl:value-of select="'X'"/>
				</DDCNumber>
				<ClassPath>
					<xsl:value-of select="../extDBList/@source"/>
				</ClassPath>
				<Hits>
					<xsl:value-of
            select="count(mo:modsCollection/mo:mods)"
          />
				</Hits>
-->        
				<xsl:for-each select="//zs:record/zs:recordData/mo:mods">
					<xsl:element name="Dokument">
						<xsl:call-template name="parseMODSdoc" />
					</xsl:element>
				</xsl:for-each>
			</Systematik>
		</xsl:if>
	</xsl:template>


  <xsl:template match="/">
    <xsl:call-template name="getMODScollection"></xsl:call-template>
<!--
    <xsl:choose>
      <xsl:when test="@format='MODS3-2'">
        <xsl:call-template name="getMODScollection"></xsl:call-template>
      </xsl:when>
    </xsl:choose>
-->
  </xsl:template>

</xsl:stylesheet> 

