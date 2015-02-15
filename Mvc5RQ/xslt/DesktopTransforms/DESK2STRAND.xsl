<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
    <xsl:output doctype-public="-//RiQuest Software//DTD JBArticle v1.0 20020724//EN" doctype-system="../dtd/JBArticle/jbarticle.dtd"/>
    
    <xsl:param name="ProjectName" select="''"/>

    <xsl:template name="substring-before-endtag">
        <xsl:param name="TestString" />
        <xsl:param name="StartTag" />

        <xsl:value-of select="substring-before($TestString, concat('[/', $StartTag, ']'))"/>
        <xsl:if test="contains(substring-before($TestString, concat('[/', $StartTag, ']')), concat('[', $StartTag,']'))">
            <xsl:value-of select="concat('[/', $StartTag, ']')"/>
            <xsl:call-template name="substring-before-endtag">
                <xsl:with-param name="TestString" select="substring-after($TestString, concat('[/', $StartTag, ']'))" />
                <xsl:with-param name="StartTag" select="$StartTag" />
            </xsl:call-template>
        </xsl:if>
    </xsl:template>
    
    <xsl:template name="substring-after-endtag">
        <xsl:param name="TestString" />
        <xsl:param name="StartTag" />
        
        <xsl:choose>
            <xsl:when test="contains(substring-before($TestString, concat('[/', $StartTag, ']')), concat('[', $StartTag,']'))">
                <xsl:call-template name="substring-after-endtag">
                    <xsl:with-param name="TestString" select="substring-after($TestString, concat('[/', $StartTag, ']'))" />
                    <xsl:with-param name="StartTag" select="$StartTag" />
                </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="substring-after($TestString, concat('[/', $StartTag, ']'))" />                    
            </xsl:otherwise>
        </xsl:choose>           
    </xsl:template>
    
    <xsl:template name="remove-duplicates">
        <xsl:param name="string" />
        <xsl:param name="newstring" />
        
        <xsl:choose>
            <xsl:when test="$string = ''">
                <xsl:value-of select="$newstring" />
            </xsl:when>
            <xsl:otherwise>
                <xsl:if test="contains($newstring, substring-before($string, ','))">
                    <xsl:call-template name="remove-duplicates">
                        <xsl:with-param name="string" select="substring-after($string, ',')" />
                        <xsl:with-param name="newstring" select="$newstring" />
                    </xsl:call-template>
                </xsl:if>
                <xsl:if test="not(contains($newstring, substring-before($string, ',')))">
                    <xsl:variable name="temp">
                        <xsl:if test="$newstring = ''">
                            <xsl:value-of select="substring-before($string, ',')" />
                        </xsl:if>
                        <xsl:if test="not($newstring = '')">
                            <xsl:value-of select="concat($newstring, ',', substring-before($string, ','))" />
                        </xsl:if>
                    </xsl:variable>
                    <xsl:call-template name="remove-duplicates">
                        <xsl:with-param name="string" select="substring-after($string, ',')" />
                        <xsl:with-param name="newstring" select="$temp" />
                    </xsl:call-template>
                </xsl:if>
            </xsl:otherwise>
        </xsl:choose>        
    </xsl:template>
    
    <xsl:template name="TrimFront">
        <xsl:param name="TargetString" />
        <xsl:param name="SubString" />
        
        <xsl:choose>
            <xsl:when test="starts-with($TargetString, $SubString)">
                <xsl:value-of select="substring($TargetString, string-length($SubString) + 1)" />
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="normalize-space($TargetString)"/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    <xsl:template name="CheckOpenTags">
        <xsl:param name="FieldSubString" />
        <xsl:param name="StartTag" />
        <xsl:param name="EndTag" />
        
        <xsl:if test="not(contains($FieldSubString, $EndTag))">
            <xsl:value-of select="$StartTag"/>
        </xsl:if>
        <xsl:call-template name="CloseOpenTags">
            <xsl:with-param name="FieldSubString">
                <xsl:choose>
                    <xsl:when test="contains($FieldSubString, $EndTag)">
                        <xsl:value-of select="substring-after($FieldSubString,$EndTag)"/>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select="$FieldSubString"/>
                    </xsl:otherwise>
                </xsl:choose>                
            </xsl:with-param>
        </xsl:call-template>        
    </xsl:template>
    
    <xsl:template name="CloseOpenTags">
        <xsl:param name="FieldSubString" />
        
        <xsl:if test="contains(string($FieldSubString),'[')">
            <xsl:variable name="Head" select="substring-before($FieldSubString,'[')" />
            <xsl:variable name="Tail" select="substring-after($FieldSubString,']')" />
            <xsl:variable name="StartTag" select="substring(substring-before(substring-after($FieldSubString,'['), ']'), 1, 2)" />
            
            <xsl:choose>
                <xsl:when test="contains($StartTag, 'fn')">
                    <xsl:call-template name="CheckOpenTags">
                        <xsl:with-param name="FieldSubString" select="concat(']',$Tail)" />
                        <xsl:with-param name="StartTag" select="$StartTag" />
                        <xsl:with-param name="EndTag" select="']'" />
                    </xsl:call-template>  
                </xsl:when>
                <xsl:when test="contains('f q qm z ', $StartTag)">
                    <xsl:call-template name="CheckOpenTags">
                        <xsl:with-param name="FieldSubString" select="$Tail" />
                        <xsl:with-param name="StartTag" select= "concat('[', $StartTag, ']')" />
                        <xsl:with-param name="EndTag" select="concat('[/', normalize-space($StartTag), ']')" />
                    </xsl:call-template>  
                </xsl:when>                
            </xsl:choose>            
        </xsl:if>
    </xsl:template>
    
    <xsl:template name="TagMarkups">
        <xsl:param name="FieldSubString" />
        <xsl:param name="StartTag" />
        <xsl:param name="EndTag" />
        
        <xsl:variable name="Tail">
            <xsl:choose>
                <xsl:when test="$StartTag='qm'">
                    <xsl:call-template name="substring-after-endtag">
                        <xsl:with-param name="TestString" select="$FieldSubString" />
                        <xsl:with-param name="StartTag" select="$StartTag" />
                    </xsl:call-template>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="substring-after($FieldSubString, $EndTag)"/>
                </xsl:otherwise>
            </xsl:choose>          
        </xsl:variable>
        <xsl:variable name="FieldValue">
            <xsl:choose>
                <xsl:when test="contains($FieldSubString, $EndTag)">
                    <xsl:choose>
                        <xsl:when test="$StartTag='qm'">
                            <xsl:call-template name="substring-before-endtag">
                                <xsl:with-param name="TestString" select="$FieldSubString" />
                                <xsl:with-param name="StartTag" select="$StartTag" />
                            </xsl:call-template>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:value-of select="normalize-space(substring-before($FieldSubString, $EndTag))"/>
                        </xsl:otherwise>
                    </xsl:choose>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="normalize-space($FieldSubString)"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <xsl:variable name="Pagination">
            <xsl:if test="($StartTag = 'fn') and contains($FieldSubString,$EndTag) and contains($FieldSubString,')')">
                <xsl:choose>
                    <xsl:when test="contains(substring-before($FieldSubString, ')'), '(')">
                        <xsl:value-of select="concat(substring-after(substring-before(substring-before($FieldSubString, '('), ')'), $EndTag), ')')"/>                      
                    </xsl:when>
                    <xsl:when test="contains(substring-before($FieldSubString, ')'), ', [fn')">
                        <xsl:value-of select="concat(substring-after(substring-before($FieldSubString, ', [fn'), $EndTag), ')')"/>                      
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select="concat(substring-after(substring-before($FieldSubString, ')'), $EndTag), ')')"/>                    
                    </xsl:otherwise>                    
                </xsl:choose>
                
            </xsl:if>                       
        </xsl:variable>                    
                
        <xsl:choose>
            <xsl:when test="$StartTag = 'f'">
                <xsl:element name="Emphasis">
                    <xsl:call-template name="ZknMarkup">
                        <xsl:with-param name="FieldSubString" select="$FieldValue" />
                    </xsl:call-template>             
                </xsl:element>
            </xsl:when>
            <xsl:when test="$StartTag = 'q'">
                <xsl:element name="Quote">
                    <xsl:call-template name="ZknMarkup">
                        <xsl:with-param name="FieldSubString">
                            <xsl:choose>
                                <xsl:when test="starts-with(normalize-space($Tail), '([fn') and contains($Tail, ')')">
                                    <xsl:value-of select="concat('&quot;', $FieldValue, '&quot;', substring-before(normalize-space($Tail), ')'), ')')"/>
                                </xsl:when>
                                <xsl:otherwise>
                                    <xsl:value-of select="concat('&quot;', $FieldValue, '&quot;')"/>
                                </xsl:otherwise>
                            </xsl:choose>                            
                        </xsl:with-param>
                    </xsl:call-template>                        
                </xsl:element>                       
            </xsl:when>
            <xsl:when test="$StartTag = 'qm'">
                <xsl:call-template name="ZknMarkup">
                    <xsl:with-param name="FieldSubString" select="concat('&quot;', $FieldValue, '&quot;')" />
                </xsl:call-template>                        
            </xsl:when>
            <xsl:when test="$StartTag = 'z '">
                <xsl:element name="Emphasis">
                    <xsl:call-template name="ZknMarkup">
                        <xsl:with-param name="FieldSubString" select="$FieldValue" />
                    </xsl:call-template>                        
                </xsl:element>                       
            </xsl:when>
            <xsl:when test="$StartTag = 'fn'">
                <xsl:element name="Citation">
                    <xsl:variable name="BibNr" select="substring-after($FieldValue, 'fn ')" />
                    <xsl:variable name="BibAbbr" select="concat(substring-before(string(document('authorFile.xml')/authors/entry[round($BibNr)]), ']'), ']')" />

                    <xsl:attribute name="Linkend">                       
                        <xsl:value-of select="concat('Bib', $BibNr)" />
                    </xsl:attribute>
                        <xsl:value-of select="concat($BibAbbr, substring-before($Pagination, ')'))" />
                </xsl:element>
            </xsl:when>            
        </xsl:choose>
        <xsl:if test="$Tail">
            <xsl:call-template name="ZknMarkup">
                <xsl:with-param name="FieldSubString">
                    <xsl:choose>
                        <xsl:when test="string($Pagination) and not($Pagination=')')">
                            <xsl:value-of select="substring-after($Tail, $Pagination) "/>
                        </xsl:when>
                        <xsl:when test="$StartTag = 'q' and starts-with(normalize-space($Tail), '([fn') and contains($Tail, ')')">
                            <xsl:value-of select="substring-after($Tail, ')') "/>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:value-of select="$Tail "/>                            
                        </xsl:otherwise>
                    </xsl:choose>                 
                </xsl:with-param>
            </xsl:call-template>                        
        </xsl:if>      
    </xsl:template>
    
    <xsl:template name="ZknMarkup">
        <xsl:param name="FieldSubString" />    
        
        <xsl:choose>
            <xsl:when test="contains($FieldSubString,'[')">
                <xsl:variable name="Head">
                    <xsl:choose>
                        <xsl:when test="contains($FieldSubString,'([fn') and not(contains(substring-before($FieldSubString, '([fn'), '['))">
                            <xsl:value-of select="substring-before($FieldSubString,'([fn')"/>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:value-of select="substring-before($FieldSubString,'[')" />
                        </xsl:otherwise>
                    </xsl:choose>
                </xsl:variable>
                <xsl:variable name="Tail" select="substring-after($FieldSubString,']')" />
                <xsl:variable name="StartTag" select="substring(substring-before(substring-after(normalize-space($FieldSubString),'['), ']'), 1, 2)" />
                
                <xsl:if test="$Head">
                    <xsl:value-of select="$Head"/>
                </xsl:if>
                <xsl:choose>
                    <xsl:when test="contains('f q qm z ', $StartTag)">
                        <xsl:call-template name="TagMarkups">
                            <xsl:with-param name="FieldSubString" select="$Tail" />
                            <xsl:with-param name="StartTag" select="$StartTag" />
                            <xsl:with-param name="EndTag" select="concat('[/', normalize-space($StartTag), ']')" />
                        </xsl:call-template>  
                    </xsl:when>
                    <xsl:when test="contains($StartTag, 'fn')">
                        <xsl:call-template name="TagMarkups">
                            <xsl:with-param name="FieldSubString" select="substring-after($FieldSubString, '[')" />
                            <xsl:with-param name="StartTag" select="$StartTag" />
                            <xsl:with-param name="EndTag" select="']'" />
                        </xsl:call-template>   
                    </xsl:when>
                </xsl:choose>              
            </xsl:when>
            <xsl:otherwise>
                <xsl:if test="string-length($FieldSubString) != 0 and not(starts-with($FieldSubString, '&quot;&quot;'))">
                    <xsl:value-of select="$FieldSubString"/>
                </xsl:if>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    <xsl:template name="SplitParagraphs">
        <xsl:param name="FieldSubString" />
        
        <xsl:variable name="Head" select="substring-before($FieldSubString, '[br]')" />
        <xsl:variable name="Tail">
            <xsl:call-template name="TrimFront">
                <xsl:with-param name="TargetString" select="substring-after($FieldSubString, '[br]')" />
                <xsl:with-param name="SubString" select="'[br]'" />
            </xsl:call-template>                
        </xsl:variable>
        <xsl:variable name="OpenTags">
            <xsl:call-template name="CloseOpenTags">
                <xsl:with-param name="FieldSubString" select="$Head" />
            </xsl:call-template>
        </xsl:variable>
        
        <xsl:if test="$Head">
            <xsl:element name="Para">
                <xsl:call-template name="ZknMarkup">
                    <xsl:with-param name="FieldSubString" select="$Head" />             
                </xsl:call-template>                
            </xsl:element>
        </xsl:if>              
        <xsl:choose>
            <xsl:when test="string($Tail)">
                <xsl:call-template name="SplitParagraphs">
                    <xsl:with-param name="FieldSubString" select="concat(string($OpenTags), $Tail)" />
                </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
                <xsl:if test="not($Head)">
                    <xsl:element name="Para">
                        <xsl:call-template name="ZknMarkup">
                            <xsl:with-param name="FieldSubString" select="concat(string($OpenTags), $FieldSubString)" />             
                        </xsl:call-template>              
                    </xsl:element>               
                </xsl:if>
            </xsl:otherwise>
        </xsl:choose>       
    </xsl:template>
    
    <xsl:template name="WriteBiblioItem">
        <xsl:param name="BibNrList"/>  
       
        <xsl:element name="BiblioItem">
            <xsl:variable name="BibId" select="normalize-space(substring-before($BibNrList, ','))" />
            
            <xsl:attribute name="Id">
                <xsl:value-of select="concat('Bib', $BibId)" />
            </xsl:attribute>
            <xsl:element name="Designator">
                <xsl:value-of select="substring-after(substring-before(document('authorFile.xml')/authors/entry[round($BibId)], ']'), '[')"/>
            </xsl:element>
            <xsl:element name="BiblioEntry">
                <xsl:variable name="EntryValue" select="normalize-space(substring-after(document('authorFile.xml')/authors/entry[round($BibId)], ']'))" />
                
                <xsl:if test="contains($EntryValue, '[RQ')">
                    <xsl:attribute name="Id">
                        <xsl:value-of select="concat('RQ', substring-before(substring-after($EntryValue, '[RQ'), ']'))"/>
                    </xsl:attribute>
                    <xsl:value-of select="substring-before($EntryValue, '[RQ')"/>
                </xsl:if>
                <xsl:if test="not(contains($EntryValue, '[RQ'))">
                    <xsl:value-of select="$EntryValue"/>                    
                </xsl:if>
            </xsl:element>
        </xsl:element>
        <xsl:if test="string-length(substring-after($BibNrList, ',')) > 0">
            <xsl:call-template name="WriteBiblioItem">
                <xsl:with-param name="BibNrList" select="substring-after($BibNrList, ',')" />
            </xsl:call-template>
        </xsl:if>
    </xsl:template>
   
    <xsl:template name="WriteBibliography">
        <xsl:variable name="BibNrList">
            <xsl:variable name="RawList">
                <xsl:apply-templates select="desktops/desktop/bullet[../@name=$ProjectName]" mode="bibliography" />
            </xsl:variable>
            
            <xsl:call-template name="remove-duplicates">
                <xsl:with-param name="string" select="normalize-space($RawList)" />
                <xsl:with-param name="newstring" select="''" />
            </xsl:call-template>           
        </xsl:variable>
              
        <xsl:element name="Bibliography">
            <xsl:element name="Title">
                <xsl:value-of select="'Bibliographie'"/>
            </xsl:element>
            <xsl:call-template name="WriteBiblioItem">
                <xsl:with-param name="BibNrList" select="concat($BibNrList, ',')" />
            </xsl:call-template>
        </xsl:element>
    </xsl:template>   
 
    <xsl:template match="/">
        <xsl:element name="Article">
            <xsl:element name="Title">
                <xsl:value-of select="$ProjectName" />
            </xsl:element>
            <xsl:apply-templates select="desktops/desktop/bullet[../@name=$ProjectName]" >
                <xsl:with-param name="Level" select="1" /> 
            </xsl:apply-templates>            
            <xsl:call-template name="WriteBibliography" />
        </xsl:element>    
    </xsl:template>
    
    <xsl:template match="bullet" mode="bibliography">
        <xsl:apply-templates mode="bibliography" />
    </xsl:template>
    
    <xsl:template match="bullet" >
        <xsl:param name="Level" />
       
        <xsl:element name="{concat('Sect', $Level)}" >
            <xsl:attribute name="Id">
                <xsl:value-of select="@name"/>                
            </xsl:attribute>
            <xsl:apply-templates>
                <xsl:with-param name="Level" select="$Level + 1" />
            </xsl:apply-templates>
        </xsl:element> 
    </xsl:template>
    
    <xsl:template match="entry">    
        <xsl:variable name="Number" select="@id" />
        <xsl:variable name="WithTitle" select="position()=1" />
        
        <xsl:apply-templates select="document('zknFile.xml')/zettelkasten/zettel[position()=$Number]">
            <xsl:with-param name="WithTitle" select="$WithTitle" />
        </xsl:apply-templates>
    </xsl:template>
    
    <xsl:template match="entry" mode="bibliography">
        <xsl:variable name="Number" select="@id" />
        
        <xsl:apply-templates select="document('zknFile.xml')/zettelkasten/zettel[position()=$Number]" mode="bibliography" />
    </xsl:template>
    
    <xsl:template match="zettel">
        <xsl:param name="WithTitle" />
        
        <xsl:apply-templates>
            <xsl:with-param name="WithTitle" select="$WithTitle" />
        </xsl:apply-templates>
    </xsl:template> 
    
    <xsl:template match="zettel" mode="bibliography">
        <xsl:if test="author">
            <xsl:value-of select="concat(author, ',')" />
        </xsl:if>               
    </xsl:template>
    
    <xsl:template match="title">
        <xsl:param name="WithTitle" />
        
        <xsl:if test="$WithTitle">
            <xsl:element name="Title">
                <xsl:value-of select="."/>
            </xsl:element>
        </xsl:if>
    </xsl:template>    
    
    <xsl:template match="content">
        <xsl:choose>
            <xsl:when test="contains(.,'[br]')">
                <xsl:call-template name="SplitParagraphs">
                    <xsl:with-param name="FieldSubString" select="." />                  
                </xsl:call-template>                
            </xsl:when>
            <xsl:otherwise>
                <xsl:element name="Para">
                    <xsl:call-template name="ZknMarkup">
                        <xsl:with-param name="FieldSubString" select="." />
                    </xsl:call-template>                
                </xsl:element>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    <xsl:template match="*">
        
    </xsl:template>       
    
</xsl:stylesheet>
