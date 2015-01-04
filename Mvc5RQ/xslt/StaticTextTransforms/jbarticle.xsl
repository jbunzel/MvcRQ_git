<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0" 
                xmlns:msxml='urn:schemas-microsoft-com:xslt' 
                xmlns:myscript='http://www.strands.de/myscript'
                exclude-result-prefixes='msxml myscript'>
  <!-- Stylesheet zur Fließtextanzeige von Dokumenten mit jbarticle.dtd -->
  <xsl:output omit-xml-declaration="yes" />

  <xsl:param name="AppBase" select="''"/>
  <xsl:param name="AppURL" select="''"/>
  <xsl:param name="SrcURL" select="''"/>
  <xsl:param name="ArticleBase" select="''"/>
  <xsl:param name="Sect" select="*"/>
  <xsl:param name="Extensions" select="'MathJax TeXVC MathTEX MathPlayer MatML2.0'"/>

  <xsl:key name="linkend-group" match="Link" use="@Linkend" />

  <xsl:param name="SmartURLEnabled" select="'false'"/>

  
  <xsl:template match="/">
    <xsl:apply-templates />
    <h2>Fußnoten</h2>
    <xsl:call-template name="generate-linklist">
      <xsl:with-param name="link-nodelist" select="$Sect//Link[generate-id()=generate-id(key('linkend-group',@Linkend)[ancestor::*=$Sect][1])]" />
    </xsl:call-template>
  </xsl:template>

  
  <xsl:template match="*[@Language='English']">
  </xsl:template>


  <xsl:template match="*[@Language='Other']">
  </xsl:template>
  
  
  <!-- Xsl-Scripts -->

  <msxml:script implements-prefix='myscript'>
    function rnd(nodeList) { return Math.random(); }
  </msxml:script>

  
  <!-- Named Templates -->

  <xsl:template name="generate_smart_url">
    <xsl:param name="location" select="''"/>
    <xsl:param name="section" select="''"/>
    <xsl:param name="paragraph" select="''"/>

    <xsl:if test="not($location='')">
      <xsl:choose>
        <xsl:when test="not($section='')">
          <xsl:choose>
            <xsl:when test="not($paragraph='')">
              <xsl:choose>
                <xsl:when test="$SmartURLEnabled='true'">
                  <xsl:value-of select="concat(substring-before($AppURL,'/default.aspx'), '/', substring-before($location, '/'), '/', $paragraph, '/', $section, '.aspx')" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="concat($AppURL,'?L=',$location,'&amp;S=',$section,'&amp;P=', $paragraph)" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:when>
            <xsl:otherwise>
              <xsl:choose>
                <xsl:when test="$SmartURLEnabled='true'">
                  <xsl:value-of select="concat(substring-before($AppURL,'/default.aspx'), '/', substring-before($location, '/'), '/', $section, '.aspx')" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="concat($AppURL,'?L=',$location,'&amp;S=',$section)" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:when>
        <xsl:otherwise>
          <xsl:choose>
            <xsl:when test="$SmartURLEnabled='true'">
              <xsl:value-of select="concat(substring-before($AppURL,'/default.aspx'), '/', substring-before($location, '/'), '/', 'start.aspx')" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="concat($AppURL,'?L=',$location,'&amp;S=1')" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
  </xsl:template>


  <xsl:template name="generate-linklist">
    <xsl:param name="link-nodelist" />

    <xsl:for-each select="$link-nodelist" >
      <xsl:if test="//Footnote[@Id=current()/@Linkend and not(current()/ancestor::*[@Language='English']) and not(@Type='Sonstiges')]">
        <xsl:apply-templates select="//Footnote[@Id=current()/@Linkend and not(current()/ancestor::*[@Language='English']) and not(@Type='Sonstiges')]" mode="linklist" />
      </xsl:if>
    </xsl:for-each>
  </xsl:template>


  <xsl:template name="generate-footnote-ref">
    <xsl:param name="EID" />

    <xsl:element name="span">
      <sup class="reference">
        <!-- ERROR: Referenzen auf gleiche Fußnoten werden unterschiedlich numeriert -->
        <xsl:value-of select="concat(count(preceding::Link[@Type='Footnote' and not(ancestor::*[@Language='English'])])+1,') ')" />
      </sup>
    </xsl:element>
  </xsl:template>


  <xsl:template name="generate-numbereditem-ref">
    <xsl:param name="EID" />
    <xsl:param name="type" />

    <xsl:element name="span">
      <xsl:attribute name="class">
        <xsl:text>lookup</xsl:text>
      </xsl:attribute>
      <xsl:choose>
        <xsl:when test="@Type='Definition' and not(string()=string(current()/@Linkend))">
          <xsl:value-of select="string()" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:variable name="nrstr">
            <xsl:for-each select="//NumberedItem[@Id=current()/@Linkend]">
              <xsl:choose>
                <xsl:when test="@Type='Definition'">
                  <xsl:text>Definition </xsl:text>
                </xsl:when>
                <xsl:when test="@Type='Satz'">
                  <xsl:text>Satz </xsl:text>
                </xsl:when>
                <xsl:when test="@Type='Lemma'">
                  <xsl:text>Lemma </xsl:text>
                </xsl:when>
                <xsl:when test="@Type='Figure'">
                  <xsl:text>Bild </xsl:text>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="''"/>
                </xsl:otherwise>
              </xsl:choose>
              <xsl:text> (</xsl:text>
              <xsl:number level="any" count="Sect1" />
              <xsl:text>.</xsl:text>
              <xsl:number level="any" count="NumberedItem[@Type=$type]" from="Sect1"/>
              <xsl:text>) </xsl:text>
            </xsl:for-each>
          </xsl:variable>

          <xsl:value-of select="$nrstr" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:element>
  </xsl:template>


  <xsl:template name = "id">
    <xsl:for-each select = "@Id">
      <a name = "{.}" ></a>
    </xsl:for-each>
  </xsl:template>
  

  <xsl:template name = "id-and-children">
    <xsl:call-template name = "id" />
    <xsl:apply-templates />
  </xsl:template>
  
  
  <!-- Document Structure Matches -->
  
  <xsl:template match = "Article/Title">
    <h1>
      <xsl:apply-templates/>
    </h1> 
  </xsl:template>


  <xsl:template match = "Trailer">
    <div class="Trailer">
      <xsl:call-template name = "id" />
      <h2>
        <xsl:text>Trailer</xsl:text>
      </h2>
      <xsl:apply-templates />
    </div>
  </xsl:template>


  <xsl:template match = "Abstract">
    <div class="Abstract">
      <xsl:call-template name = "id" />
      <h2>
        <xsl:text>Abstract</xsl:text>
      </h2>
      <xsl:apply-templates />
    </div>    
  </xsl:template>


  <xsl:template match = "Motto">
    <div class="Motto">
      <xsl:call-template name = "id" />
      <xsl:apply-templates />
    </div>  
  </xsl:template>


  <xsl:template match = "Sect1|Sect2|Sect3|Sect4|Appendix|SeeAlso|Bibliography">
    <xsl:element name="div">
      <xsl:attribute name="class">
        <xsl:value-of select="name()" />
      </xsl:attribute>
      <xsl:call-template name = "id-and-children"/>
    </xsl:element>
  </xsl:template>


  <xsl:template match = "Sect1/Title">
    <h2>
      <xsl:apply-templates/>
    </h2> 
  </xsl:template>  


  <xsl:template match = "Sect2/Title">
    <h3>
      <xsl:apply-templates/>
    </h3>
  </xsl:template>


  <xsl:template match = "Sect3/Title">
    <h4>
      <xsl:apply-templates/>
    </h4>
  </xsl:template>
  

  <xsl:template match = "Sect4/Title">
    <h5>
      <xsl:apply-templates/>
    </h5>
  </xsl:template>


  <xsl:template match="NumberedItem">
    <xsl:variable name="type" select="@Type" />
    
    <xsl:element name="div">
      <xsl:attribute name="class">
        <xsl:value-of select="@Type" />
      </xsl:attribute>
      <xsl:element name="span">
        <xsl:attribute name="class">
          <xsl:text>NumberedItemCount</xsl:text>
        </xsl:attribute>
        <xsl:number level="any" count="Sect1" />
        <xsl:text>. </xsl:text>
        <xsl:number level="any" count="NumberedItem[@Type=$type]" from="Sect1"/>
      </xsl:element>
      <xsl:apply-templates select="child::*" />
    </xsl:element>
  </xsl:template>

  
  <xsl:template match="Footnote">
    <!-- display footnotes only by link activation -->
  </xsl:template>


  <xsl:template match="Footnote" mode="inline">
    <xsl:param name="text-prefix" select="''" />
    
    <!-- Inline-Fußnoten sind bei Fließtextanzeige des Dokuments immer sichtbar. -->
    <xsl:value-of select="$text-prefix" />
    <xsl:apply-templates select="." mode="linklist" />
  </xsl:template>


  <xsl:template match="Footnote" mode="linklist">
    <xsl:element name="div">
      <xsl:if test="@Type='Footnote'">
        <xsl:attribute name="class">
          <xsl:text>Linklist Footnote</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <xsl:attribute name="Id">
        <xsl:value-of select="concat('NILINK',@Id)" />
      </xsl:attribute>
      <xsl:element name="span">
        <xsl:if test="@Type='Footnote'">
          <!-- ERROR: Fußnoten, die mehrfach referenziert sind werden falsch gezählt
          <xsl:value-of select="count(//Link[@Linkend=current()/@Id][1]/preceding::Link[@Type='Footnote' and not(ancestor::*[@Language='English'])])+1" />
          -->
        </xsl:if>
        <xsl:choose>
          <xsl:when test="child::*[1][name()='Para']">
            <xsl:apply-templates />
          </xsl:when>
          <xsl:otherwise>
            <xsl:element name="p">
              <xsl:apply-templates />
            </xsl:element>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:element>
    </xsl:element>
  </xsl:template>


  <xsl:template match="Link">
  <!-- TODO: Implement Id based references and SmartURLs. -->    
    <xsl:choose>
      <xsl:when test="//Sect1[@Id=current()/@Linkend]">
        <xsl:variable name="count" select="count(//Sect1[@Id=current()/@Linkend]/preceding-sibling::Sect1)"/>
        <a href="{$AppURL}?L={$SrcURL}&amp;S={$count+1}&amp;P=0">
          <xsl:apply-templates/>
        </a>
      </xsl:when>
      <xsl:when test="//Sect2[@Id=current()/@Linkend]">
        <xsl:variable name="count2" select="count(//Sect2[@Id=current()/@Linkend]/preceding-sibling::Sect2)"/>
        <xsl:variable name="count1" select="count(//Sect2[@Id=current()/@Linkend]/../preceding-sibling::Sect1)"/>
        <a href="{$AppURL}?L={$SrcURL}&amp;S={$count1+1}&amp;P={$count2+1}">
          <xsl:apply-templates/>
        </a>
      </xsl:when>
      <xsl:when test="//NumberedItem[@Id=current()/@Linkend]">
        <xsl:variable name="EID">
          <xsl:value-of select="generate-id()"/>
        </xsl:variable>

        <xsl:variable name="type">
          <xsl:choose>
            <xsl:when test="@Type">
              <xsl:value-of select="@Type"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'Special'"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:variable>
        
        <xsl:call-template name="generate-numbereditem-ref">
          <xsl:with-param name="EID" select="$EID" />
          <xsl:with-param name="type" select="$type" />
        </xsl:call-template>
        <xsl:element name="span">
          <xsl:attribute name="ID">
            <xsl:value-of select="$EID" />
          </xsl:attribute>
          <xsl:attribute name="class">
            <xsl:value-of select="concat('Linklist ',$type)" />
          </xsl:attribute>
        </xsl:element>
      </xsl:when>
      <xsl:when test="//Footnote[@Id=current()/@Linkend]">
        <xsl:variable name="EID">
          <xsl:value-of select="generate-id()"/>
          <xsl:value-of select='myscript:rnd(.)' />
        </xsl:variable>

        <xsl:choose>
          <xsl:when test="@Type='Sonstiges'">
            <xsl:apply-templates select="//Footnote[@Id=current()/@Linkend]" mode="inline" >
              <xsl:with-param name="text-prefix" select="." />
            </xsl:apply-templates>
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name="generate-footnote-ref">
              <xsl:with-param name="EID" select="$EID" />
            </xsl:call-template>
            <xsl:element name="span">
              <xsl:attribute name="ID">
                <xsl:value-of select="$EID" />
              </xsl:attribute>
              <xsl:attribute name="class">
                <xsl:text>Linklist Footnote</xsl:text>
              </xsl:attribute>
            </xsl:element>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:otherwise>
        <a href="#{@Linkend}">
          <xsl:apply-templates/>
        </a>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <xsl:template match = "DLink">
    <a href="{@Document}">
      <xsl:apply-templates/>
    </a>
  </xsl:template>


  <xsl:template match = "ALink">
    <a href="{@Article}">
      <xsl:apply-templates/>
    </a>
  </xsl:template>


  <xsl:template match = "ELink">
    <a href="{@URL}">
      <xsl:apply-templates/>
    </a>
  </xsl:template>


  <xsl:template match = "ULink">
    <a href="{@URL}">
      <xsl:apply-templates/>
    </a>
  </xsl:template>


  <xsl:template match = "Graphic">
    <xsl:param name="base" select="$AppBase" />
    
    <div class="Graphic">
      <img src="{$base}{@FileRef}" height="{@Depth}" width="{@Width}" alt="{@Alt}" />
    </div>
  </xsl:template>


  <xsl:template match = "Graphic[@Id='In1']">
    <div class="Graphic">
      <xsl:text>NOT SUPPORTED</xsl:text>
      <iframe src="{@FileRef}" height="{@Depth}" width="{@Width}">
        Embedded HTML Object
      </iframe>
    </div>
  </xsl:template>


  <xsl:template match = "InlineGraphic">
    <xsl:param name="base" select="$AppBase" />

    <span class="InlineGraphic">
      <img src="{$base}{@FileRef}" height="{@Depth}" width="{@Width}" alt="{@Alt}" />
    </span>
  </xsl:template>


  <xsl:template match = "Figure">
    <div class="Figure">
      <xsl:call-template name = "id-and-children"/>
    </div>
  </xsl:template>


  <xsl:template match = "Figure/Title">
    <h2>
      <xsl:apply-templates/>
    </h2>
  </xsl:template>


  <xsl:template match = "Citation">
    <a href="#{@Linkend}">
      <xsl:apply-templates/>
    </a>
  </xsl:template>


  <xsl:template match = "Quote">
    <span class="Quote">
      <xsl:apply-templates />
    </span>
  </xsl:template>

  
  <xsl:template match ="Math">
        <xsl:apply-templates select="InlineGraphic" />
  </xsl:template>
  
  
  <xsl:template match="Object">
    <xsl:choose>
      <xsl:when test="@Type='Frame'">
        <xsl:element name="iframe">
          <xsl:attribute name="src">
            <xsl:value-of select="concat(substring-before($ArticleBase,'/MyArticles'),.)" />
          </xsl:attribute>
          <xsl:attribute name="Class">
            <xsl:text>frameclass</xsl:text>
          </xsl:attribute>
        </xsl:element>
      </xsl:when>
      <xsl:when test="@Type='embeddedPDF'">
        <xsl:variable name="EID">
          <xsl:value-of select="generate-id()"/>
        </xsl:variable>

        <xsl:element name="div">
          <xsl:attribute name="id">
            <xsl:value-of select="$EID" />
          </xsl:attribute>
        </xsl:element>
        <xsl:element name="div">
          <xsl:attribute name="id">
            <xsl:value-of select="concat('link',$EID)" />
          </xsl:attribute>
          <xsl:attribute name="class">
            <xsl:text>embeddedPDF</xsl:text>
          </xsl:attribute>
          <xsl:element name="a">
            <xsl:attribute name="href">
              <xsl:text>#</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="onclick">
              <xsl:text>embedPDF("</xsl:text>
              <xsl:value-of select="concat(substring-before($ArticleBase,'/MyArticles'),.)" />
              <xsl:text>", "</xsl:text>
              <xsl:value-of select="$EID" />
              <xsl:text>"); return false;</xsl:text>
            </xsl:attribute>
            <xsl:text>lesen...</xsl:text>
          </xsl:element>
        </xsl:element>
      </xsl:when>
      <xsl:otherwise>
        <xsl:element name="div">
          <xsl:attribute name="class">
            <xsl:text>Object</xsl:text>
          </xsl:attribute>
          <xsl:value-of disable-output-escaping="yes" select="."/>
        </xsl:element>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  
  <xsl:template match="D3Graph">
    <xsl:param name="base" select="$AppBase" />
    
    <xsl:element name="div">
      <xsl:attribute name="id">
        <xsl:value-of select="@Id" />
      </xsl:attribute>
      <xsl:attribute name="class">
        <xsl:value-of select="concat('d3graph ', @Type, '-class ', @Id, '-class')" />
      </xsl:attribute>
      <xsl:element name="link">
        <xsl:attribute name="href">
          <xsl:value-of select="concat($base, 'D3Graph/css/d3graph.css')" />
        </xsl:attribute>
        <xsl:attribute name="type">
          <xsl:value-of select="'text/css'" />
        </xsl:attribute>
        <xsl:attribute name="rel">
          <xsl:value-of select="'stylesheet'" />
        </xsl:attribute>
      </xsl:element>
      <xsl:element name="link">
        <xsl:attribute name="href">
          <xsl:value-of select="concat($ArticleBase, 'D3Graph/css/', @Type, '.css')" />
        </xsl:attribute>
        <xsl:attribute name="type">
          <xsl:value-of select="'text/css'" />
        </xsl:attribute>
        <xsl:attribute name="rel">
          <xsl:value-of select="'stylesheet'" />
        </xsl:attribute>
      </xsl:element>
      <xsl:element name="script">
        <xsl:attribute name="type">
          <xsl:value-of select="'text/javascript'" />
        </xsl:attribute>
        <xsl:choose>
          <xsl:when test="D3Struct/@Storage = 'local'">
            <xsl:variable name="json" select="normalize-space(D3Struct)" />
            <xsl:value-of select="concat('D3GraphLocal(&quot;', $json, '&quot;, &quot;', @Id, '&quot;, &quot;', D3Struct/@Type, '&quot;);')" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="concat('D3GraphRemote(&quot;',$base, 'D3Graph/', @Id, '&quot;, &quot;', @Id, '&quot;, &quot;', D3Struct/@Type, '&quot;);')" />
          </xsl:otherwise>
        </xsl:choose>
      </xsl:element>
    </xsl:element>
  </xsl:template>


  <!-- SeeAlso as a table. -->

  <xsl:template match = "SeeAlso">
    <table>
      <!-- Headers -->
      <tr>
        <td>
          <b>Description</b>
        </td>
        <td>
          <b>Location</b>
        </td>
        <td>
          <b>Source</b>
        </td>
      </tr>
      <xsl:apply-templates/>
    </table>
  </xsl:template>


  <xsl:template match = "SeeAlsoItem">
    <tr>
      <xsl:apply-templates/>
    </tr>
  </xsl:template>


  <xsl:template match = "Description|Location|Source">
    <td>
      <xsl:apply-templates/>
    </td>
  </xsl:template>

  <!-- End of SeeAlso as a table -->

  
  <xsl:template match = "Appendix/Title">
    <h2>
      <xsl:apply-templates/>
    </h2>
  </xsl:template>


  <xsl:template match = "Bibliography/Title">
    <h2>
      <xsl:apply-templates/>
    </h2>
  </xsl:template>

  
  <xsl:template match = "BiblioItem">
    <p>
      <xsl:call-template name = "id-and-children"/>
    </p>
  </xsl:template>


  <xsl:template match = "Designator">
    <b>
      <xsl:text>[</xsl:text>
      <xsl:copy-of select = "node()" />
      <xsl:text>] </xsl:text>
    </b>
  </xsl:template>


  <xsl:template match = "BiblioEntry">
    <xsl:apply-templates />
    <xsl:if test="@Id" >
      <xsl:variable name="docno" select="substring-after(@Id,'RQ')" />
      <a href="http://www.riquest.de/rqitems/{$docno}" title="catalog">
        <img src="images/catalog.gif" alt="catalog" width="8" height="8" />
      </a>
    </xsl:if>
  </xsl:template>


  <xsl:template match = "Note">
    <div class="Note">
      <xsl:apply-templates/>
    </div>
  </xsl:template>


  <xsl:template match = "VariableList">
    <p>
      <xsl:apply-templates/>
    </p>
  </xsl:template>


  <xsl:template match = "VariableList/Title">
    <h2>
      <xsl:apply-templates/>
    </h2>
  </xsl:template>


  <xsl:template match = "VarListEntry">
    <xsl:apply-templates/>
  </xsl:template>


  <xsl:template match = "Term">
    <xsl:apply-templates/>
    <br/>
  </xsl:template>


  <xsl:template match = "VarListEntry/ListItem">
    <xsl:apply-templates/>
    <br/>
    <br/>
  </xsl:template>


  <xsl:template match = "PubDate">
    <p>
      <i>
        Publication Date: <xsl:copy-of select="node()" />
      </i>
    </p>
  </xsl:template>


  <xsl:template match = "LastModDate">
    <p>
      <i>
        Last Modified Date: <xsl:copy-of select="node()" />
      </i>
    </p>
  </xsl:template>


  <xsl:template match = "Copyright">
    <p>
      <i>
        Copyright (c) 2011, by <xsl:copy-of select="node()" />
      </i>
    </p>
  </xsl:template>


  <!-- HTML-Element Matches -->

  <xsl:template match = "Para">
    <p>
      <xsl:apply-templates/>
    </p>
  </xsl:template>

  
  <xsl:template match = "ItemizedList">
    <xsl:apply-templates select="Title"/>
    <ul>
      <xsl:apply-templates select="ListItem"/>
    </ul>
  </xsl:template>
  

  <xsl:template match = "ItemizedList/Title">
    <h2>
      <xsl:apply-templates/>
    </h2>
  </xsl:template>  
  

  <xsl:template match = "OrderedList">
    <xsl:apply-templates select="Title"/>
    <ol>
      <xsl:apply-templates select="ListItem"/>
    </ol>
  </xsl:template>
  

  <xsl:template match = "OrderedList/Title">
    <h2>
      <xsl:apply-templates/>
    </h2>
  </xsl:template>  
  

  <xsl:template match = "ListItem">
    <li>
      <xsl:apply-templates/>
    </li>
  </xsl:template>

  
  <xsl:template match = "Emphasis">
    <i>
      <xsl:apply-templates/>
    </i>
  </xsl:template>


  <xsl:template match = "Strong">
    <b>
      <xsl:apply-templates/>
    </b>
  </xsl:template>


  <xsl:template match = "Greek">
    <span class="Greek">
      <xsl:apply-templates/>
    </span>
  </xsl:template>


  <xsl:template match = "TT">
    <tt>
      <xsl:apply-templates/>
    </tt>
  </xsl:template>


  <xsl:template match = "Underscore">
    <u>
      <xsl:apply-templates/>
    </u>
  </xsl:template>


  <xsl:template match = "Subscript">
    <sub>
      <xsl:apply-templates/>
    </sub>
  </xsl:template>


  <xsl:template match = "Superscript">
    <sup>
      <xsl:apply-templates/>
    </sup>
  </xsl:template>


  <xsl:template match = "LiteralLayout">
    <pre>
      <xsl:apply-templates/>
    </pre>
  </xsl:template>


  <xsl:template match = "ProgramListing">
    <pre>
      <xsl:apply-templates/>
    </pre>
  </xsl:template>


  <xsl:template match= "TABLE">
    <p>
      <table class="content" border="1" cellpadding="10" cellspacing="0" align="center">
        <xsl:apply-templates />
      </table>
    </p>  
  </xsl:template>
  

  <xsl:template match= "TABLE[@class='format']">
    <table border="0" cellpadding="0" cellspacing="0">
      <xsl:apply-templates />
    </table>
  </xsl:template>
    

  <xsl:template match = "CAPTION">
    <b>
      <xsl:apply-templates />
    </b> 
  </xsl:template>
    

  <xsl:template match = "THEAD|TFOOT|TBODY|COLGROUP|COL|TR|TH|TD">
    <xsl:copy>
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates/>
    </xsl:copy>
  </xsl:template>
 

  <xsl:template match = "Author">
    <p>
      <xsl:call-template name = "id-and-children" />
    </p>
  </xsl:template>
  

  <xsl:template match = "FirstName">
    <xsl:copy-of select = "node()" /><xsl:text> </xsl:text>
  </xsl:template>
  

  <xsl:template match = "Surname">
    <xsl:copy-of select = "node()" />
  </xsl:template>
  

  <xsl:template match = "JobTitle">
    <br/>
    <xsl:copy-of select = "node()" />
  </xsl:template>
  

  <xsl:template match = "OrgName">
    <br/>
    <i>
      <xsl:copy-of select = "node()" />
    </i>
  </xsl:template>
  

  <xsl:template match = "Address">
    <br/>
      <xsl:apply-templates select="Street" />
      <xsl:apply-templates select="POB" />    
    <br/>
      <xsl:apply-templates select="Postcode" />
      <xsl:apply-templates select="City" />
      <xsl:apply-templates select="State" />
      <xsl:apply-templates select="Country" />
      <xsl:apply-templates select="Phone" />  
      <xsl:apply-templates select="Fax" />
      <xsl:apply-templates select="Email" />
  </xsl:template>
  

  <xsl:template match = "Street">
    <xsl:copy-of select = "node()" />
  </xsl:template>
  

  <xsl:template match = "POB">
    <xsl:if test="//Street">, </xsl:if>
    <xsl:text>P.O. </xsl:text>
    <xsl:copy-of select = "node()" />
  </xsl:template>
  

  <xsl:template match = "Postcode">
    <xsl:copy-of select = "node()" />
  </xsl:template>
  

  <xsl:template match = "City">
    <xsl:if test="//Postcode">
      <xsl:text> </xsl:text>
    </xsl:if>
    <xsl:copy-of select = "node()" />
  </xsl:template>
  

  <xsl:template match = "State">
    <xsl:if test="//City">, </xsl:if>
    <xsl:if test="not(//City)"><br/></xsl:if>
    <xsl:copy-of select = "node()" />
  </xsl:template>
  

  <xsl:template match = "Country">
    <br/><xsl:copy-of select = "node()" />
  </xsl:template>
  

  <xsl:template match = "Phone">
    <br/>phone: <xsl:copy-of select = "node()" />
  </xsl:template>
  

  <xsl:template match = "Fax">
    <br/>fax: <xsl:copy-of select = "node()" />
  </xsl:template>
  

  <xsl:template match = "Email">
    <br/>e-mail: <xsl:copy-of select = "node()" />
  </xsl:template>

</xsl:stylesheet>
