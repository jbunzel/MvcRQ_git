<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  
  <xsl:template match="/"> 
    <root> 
    <xsl:apply-templates />
    </root>
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

  <xsl:template match = "Article/Title">
<!--    
    <h1>
      <xsl:apply-templates/>
    </h1>
-->    
  </xsl:template>
  
  <xsl:template match = "Trailer|Abstract|Motto|Sect1|Sect2|Sect3|Sect4|Appendix|Bibliography|SeeAlso">
    <div>
      <xsl:call-template name = "id-and-children"/>
    </div>
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

  <xsl:template match = "Trailer">
    <h2>
      <xsl:text>Trailer</xsl:text>
    </h2>  
    <p>
      <xsl:apply-templates />
    </p>
  </xsl:template>
 
  <xsl:template match = "Abstract">
    <h2>
      <xsl:text>Abstract</xsl:text>
    </h2>  
    <p>
      <i>
        <xsl:apply-templates />
      </i>
    </p>
  </xsl:template>
   
  <xsl:template match = "Motto">
    <h3>
      <xsl:text>Motto</xsl:text>
    </h3>  
    <p>
      <xsl:apply-templates />
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

  <xsl:template match = "Para">
    <p class="comment">
      <xsl:apply-templates/>
    </p> 
  </xsl:template>

  <xsl:template match = "Graphic">
    <div>
	    <img src="{@FileRef}" height="{@Depth}" width="{@Width}" alt="{@Alt}" />
    </div>
  </xsl:template>

  <xsl:template match = "Graphic[@Id='In1']">
    <div>
	    <xsl:text>NOT SUPPORTED</xsl:text>
      <iframe src="{@FileRef}" height="{@Depth}" width="{@Width}">
        Embedded HTML Object
      </iframe>
    </div>
  </xsl:template>

  <xsl:template match = "InlineGraphic">
    <img src="{@FileRef}" height="{@Depth}" width="{@Width}" align="{@Align}" alt="{@Alt}" />
  </xsl:template>
  
  <xsl:template match= "TABLE">
    <p>
      <table border="1" cellpadding="10" cellspacing="0" align="center">
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
 
  <xsl:template match = "PubDate">
    <p align="left">
      <i>
        <font size="-1">
          Publication Date: <xsl:copy-of select="node()" />
        </font>
      </i>
    </p>
  </xsl:template>

  <xsl:template match = "LastModDate">
    <p align="left">
      <i>
        <font size="-1">
          Last Modified Date: <xsl:copy-of select="node()" />
        </font>
      </i>
    </p>
  </xsl:template>

  <xsl:template match = "Copyright">
    <p align="left">
      <i>
        <font size="-1">
          Copyright (c) 2002, by <xsl:copy-of select="node()" />
        </font>
      </i>  
    </p>
  </xsl:template>
  
  <xsl:template match = "Author">
    <p align="center">
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
      <a href="../riquest/default.aspx?tabindex=0&amp;tabitem=0&amp;QRY=$access${$docno}" target="riquest" title="catalog">
        <img src="images/catalog.gif" align="top" alt="catalog" width="15" height="25" border="0" />
      </a>
    </xsl:if>    
  </xsl:template>

  <xsl:template match = "Note">
    <div>
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
    <font color="#1C6A00">
      <xsl:apply-templates/>
    </font>
    <br/>
  </xsl:template>
  
  <xsl:template match = "VarListEntry/ListItem">
    <xsl:apply-templates/>
    <br/>
    <br/>
  </xsl:template>
  
  <xsl:template match = "LiteralLayout">
    <pre>
      <xsl:apply-templates/>
    </pre>
  </xsl:template>

  <xsl:template match = "ProgramListing">
    <pre>
      <font color="#339900">
        <xsl:apply-templates/>
      </font>
    </pre>
  </xsl:template>
  
  <xsl:template match = "Figure">
    <div>
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
  
  <xsl:template match = "Quote">
    <p>
      <font size="-1">
        <i>
          <xsl:apply-templates />
        </i>
      </font>
    </p>  
  </xsl:template>
  
  <xsl:template match = "Greek">
    <font face="Symbol" size="+1">
      <xsl:apply-templates/>
    </font>
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
      <font size="1">
        <xsl:apply-templates/>
      </font>
    </sub>
  </xsl:template>
  
  <xsl:template match = "Superscript">
    <sup>
      <font size="1">
        <xsl:apply-templates/>
      </font>
    </sup>
  </xsl:template>
  
  <xsl:template match = "Link">
    <a href="#{@Linkend}">
      <xsl:apply-templates/>
    </a>
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
    <a href="{@URL}" target="_blank">
      <xsl:apply-templates/>
    </a>
  </xsl:template>
  
  <xsl:template match = "ULink">
    <a href="{@URL}">
      <xsl:apply-templates/>
    </a>
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
  
</xsl:stylesheet>
