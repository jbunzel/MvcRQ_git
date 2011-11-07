A Personal Digital Library is a system to manage private collections of books, journals, articles, newspaper clippings, video & audio recordings, still pictures and web links.

It provides functions to
- search, browse, add & edit the descriptions of collection items,
- render collection items available in digital formats,
- access remote databases containing item descriptions of related collections.

Advanced functions under development are:
- integrating the web user interface with the API parts of the application into the same controller logic,
  (adapting code from Omar Al Zabir: Build Truly RESTful API and Website using Same ASP.NET MVC Code (http://www.codeproject.com/KB/aspnet/aspnet_mvc_restapi.aspx) );
- identifying each collection item by a unique URL;
- presenting collection items through common APIs used in the library community (f. e. SRU, OpenURL, unAPI, CoinS);
- integrating the collection into the Semantic Web.

MvcRQ is work in progress to port the existing (although not complete) RiQuest Digital Library application (http://www.riquest.de) to the MVC-Architecture.

It consists of the following Modules:
- Sub-directory MvcRQ contains the web user interface according to MVC-Architecture. HTML is to a large extent generated via XSLT-Transformations, which optionally can be executed on server- or client-side.
- Sub-directory MvcRQ.Tests contains Unit Tests (presently not used).
- Sub-directory MVPRestApiLib contains base functions of the truly restful api (adapted code from the Al Zabir article).
- Sub-directory RQLib contains the business logic (legacy code from the existing RiQuest application).
- RQState contains baisc functions for state management.

MvcRQ is a VS 10 / .NET 4.0 application with code in VB, C#, JavaScript & JQuery.