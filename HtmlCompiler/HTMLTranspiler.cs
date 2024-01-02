using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace HtmlCompiler
{
    public static class HTMLTranspiler
    {
        const string cssDefaultStyle =
            "html, body {height: 100%; margin: 0;} body {font-family: Arial, sans-serif; display: flex; flex-direction: column;} header {position: sticky; top: 0; background-color: #f4f4f4; padding: 10px; display: flex; justify-content: space-between; align-items: center;} header a {color: #444444; text-decoration: none;} header img {max-width: 120px;} nav ul {list-style-type: none; padding: 0;} nav ul li {display: inline; margin-right: 20px;} .search-bar {border: 1px solid #444444; padding: 5px;} .btn {cursor: pointer; padding: 10px 15px; margin-left: 10px;} .btn-outline {border: 1px solid #444444; background-color: transparent;} .btn-outline:hover {opacity: 70%;} .btn-filled {background-color: #444444; color: white;} .btn-filled:hover {opacity: 70%;} .btn-none {border: none; background-color: transparent;} main {padding: 20px 30%; flex: 1 0 auto;} main a, span, h1, h2, h3 {color: #444444;} main img {max-width: 100%;} footer {background-color: #f4f4f4; padding: 10px; text-align: center; flex-shrink: 0;}";
        const string cssDarkDefaultStyle =
            "html, body {height: 100%; margin: 0; background-color:Con #121212; color: #ffffff;} body {font-family: Arial, sans-serif; display: flex; flex-direction: column;} header {position: sticky; top: 0; background-color: #2c2c2c; padding: 10px; display: flex; justify-content: space-between; align-items: center;} header a {color: #ffffff; text-decoration: none;} header img {max-width: 120px;} nav ul {list-style-type: none; padding: 0;} nav ul li {display: inline; margin-right: 20px;} .search-bar {border: 1px solid #ffffff; padding: 5px;} .btn {cursor: pointer; padding: 10px 15px; margin-left: 10px;} .btn-outline {border: 1px solid #ffffff; color: #ffffff; background-color: transparent;} .btn-outline:hover {opacity: 70%;} .btn-filled {background-color: #ffffff; color: #121212;} .btn-filled:hover {opacity: 70%;} .btn-none {border: none; background-color: transparent;} main {padding: 20px 30%; flex: 1 0 auto;} main a, span, h1, h2, h3 {color: #ffffff;} main img {max-width: 100%;} footer {background-color: #2c2c2c; padding: 10px; text-align: center; flex-shrink: 0;}";

        public static string Transpile(string[]? lines)
        {
            var htmlDoc = new HtmlDocument();
            var doctype = htmlDoc.CreateComment("<!DOCTYPE html>");
            htmlDoc.DocumentNode.AppendChild(doctype);

            var htmlNode = HtmlNode.CreateNode("<html></html>");
            htmlNode.Attributes.Add("lang", "pt");
            htmlDoc.DocumentNode.AppendChild(htmlNode);

            var headNode = HtmlNode.CreateNode("<head></head>");
            var charset = htmlDoc.CreateComment("<meta charset=\"UTF-8\">");
            var viewport = htmlDoc.CreateComment("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            var styleNode = HtmlNode.CreateNode($"<style>{cssDefaultStyle}</style>");
            headNode.AppendChild(charset);
            headNode.AppendChild(viewport);
            headNode.AppendChild(styleNode);

            var bodyNode = HtmlNode.CreateNode("<body></body>");
            htmlNode.AppendChild(headNode);
            htmlNode.AppendChild(bodyNode);

            var lineIndex = 1;
            foreach (var line in lines)
            {
                var configMatch = Regex.Match(line, @"^configura[çc][õoã](o|es)(.*):(.*)");
                var headerMatch = Regex.Match(line, @"^cabe[çc]alho(.*):(.*)");
                var mainMatch = Regex.Match(line, @"^corpo(.*):(.*)");
                var footerMatch = Regex.Match(line, @"^rodap[ée](.*):(.*)");

                //Console.WriteLine(line);

                if (configMatch.Success)
                {
                    for (var configLineIndex = lineIndex; configLineIndex < lines.Length; configLineIndex++)
                    {
                        var configLine = lines[configLineIndex];
                        var titleMatch = Regex.Match(configLine, @"^\s\st[ií]tulo(.*):(.*)");
                        var langMatch = Regex.Match(configLine, @"^\s\sidioma(.*):(.*)");
                        var themeMatch = Regex.Match(configLine, @"^\s\stema(.*):(.*)");

                        if (titleMatch.Success)
                        {
                            var title = titleMatch.Groups[2].Value.Trim();
                            title = title.EndsWith(";") ? title.Substring(0, title.Length - 1) : title;
                            var titleNode = HtmlNode.CreateNode($"<title>{title}</title>");
                            headNode.AppendChild(titleNode);
                        }
                        else if (langMatch.Success)
                        {
                            var lang = langMatch.Groups[2].Value.Trim();
                            RemoveQuotes(ref lang);
                            lang = lang.EndsWith(";") ? lang.Substring(0, lang.Length - 1) : lang;
                            htmlNode.Attributes.First(attribute => attribute.Name == "lang").Value = lang;
                        }
                        else if (themeMatch.Success)
                        {
                            var theme = themeMatch.Groups[2].Value.Trim();
                            theme = theme.EndsWith(";") ? theme.Substring(0, theme.Length - 1) : theme;
                            RemoveQuotes(ref theme);
                            if (theme == "padrão-escuro")
                            {
                                styleNode.InnerHtml = cssDarkDefaultStyle;
                            }
                        }
                        else if (!configLine.StartsWith(" ") || string.IsNullOrWhiteSpace(configLine) || configLineIndex == lines.Length - 1)
                        {
                            //Console.WriteLine($"Saiu da configuração na linha {configLineIndex + 1}: {configLine}");
                            break;
                        }
                    }
                }
                else if (headerMatch.Success)
                {
                    var headerNode = HtmlNode.CreateNode("<header></header>");
                    bodyNode.AppendChild(headerNode);

                    //Console.WriteLine("Entrou no cabeçalho na linha " + lineIndex);

                    for (var headerLineIndex = lineIndex; headerLineIndex < lines.Length; headerLineIndex++)
                    {
                        var headerLine = lines[headerLineIndex];
                        var headerEndMatch = Regex.Match(headerLine, @"^\S");
                        //var headerEndMatch = Regex.Match(headerLine, @"^(?!(cabeçalho:|  [^:]+:.*|    [^:]+:.*|      [^:]+:.*|    - .*|        - .*)).+$");

                        var positionMatch = Regex.Match(headerLine, @"^\s\sposi[çc][ãa]o(.*):(.*)");
                        var imageMatch = Regex.Match(headerLine, @"^\s\simagem(.*):(.*)");
                        var navMatch = Regex.Match(headerLine, @"^\s\snav((ega[çc][ãa]o)|)(.*):(.*)");
                        var searchMatch = Regex.Match(headerLine, @"^\s\sbusca(.*):(.*)");
                        var buttonMatch = Regex.Match(headerLine, @"^\s\sbot[ãa]o(.*):(.*)");
                        var groupMatch = Regex.Match(headerLine, @"^\s\sgrupo(.*):(.*)");

                        //Console.WriteLine("headerLine: " + headerLine);

                        if (positionMatch.Success)
                        {

                        }
                        else if (imageMatch.Success)
                        {
                            AppendImage(headerNode, headerLineIndex, lines, 2, imageMatch);
                        }
                        else if (navMatch.Success)
                        {
                            AppendNav(headerNode, headerLineIndex, lines, 2, navMatch);
                        }
                        else if (groupMatch.Success)
                        {
                            AppendGroup(headerNode, headerLineIndex, lines, 2, groupMatch);
                        }
                        else if (buttonMatch.Success)
                        {
                            AppendButton(headerNode, headerLineIndex, lines, 2, buttonMatch);
                        }
                        else if (searchMatch.Success)
                        {
                            AppendSearchBar(headerNode, headerLineIndex, lines, 2, searchMatch);
                        }
                        else if (headerEndMatch.Success)
                        {
                            //Console.WriteLine($"Match com regex de fim do cabeçalho na linha {headerLineIndex + 1}: {headerLine}");
                            break;
                        }
                    }
                    //Console.WriteLine("Saiu do cabeçalho");
                }
                else if (mainMatch.Success)
                {
                    var isMainTitle = true;
                    var mainNode = HtmlNode.CreateNode("<main></main>");
                    bodyNode.AppendChild(mainNode);

                    for (var mainLineIndex = lineIndex; mainLineIndex < lines.Length; mainLineIndex++)
                    {
                        var mainLine = lines[mainLineIndex];
                        var mainEndMatch = Regex.Match(mainLine, @"^\S");

                        var articleMatch = Regex.Match(mainLine, @"^\s\sartigo(.*):(.*)");
                        var sectionMatch = Regex.Match(mainLine, @"^\s\sse[çc][ãa]o(.*):(.*)");
                        var titleMatch = Regex.Match(mainLine, @"^\s\st[ií]tulo(.*):(.*)");
                        var subtitleMatch = Regex.Match(mainLine, @"^\s\ssubt[íi]tulo(.*):(.*)");
                        var paragraphMatch = Regex.Match(mainLine, @"^\s\sparagrafo(.*):(.*)");
                        var imageMatch = Regex.Match(mainLine, @"^\s\simagem(.*):(.*)");

                        //Console.WriteLine("mainLine: " + mainLine);

                        if (articleMatch.Success)
                        {
                            AppendArticle(mainNode, mainLineIndex, lines, 2, articleMatch);
                        }
                        else if (sectionMatch.Success)
                        {
                            AppendSection(mainNode, mainLineIndex, lines, 2, sectionMatch);
                        }
                        else if (titleMatch.Success)
                        {
                            AppendTittle(titleMatch, mainNode, isMainTitle);
                            isMainTitle = false;
                        }
                        else if (subtitleMatch.Success)
                        {
                            AppendSubtitle(subtitleMatch, mainNode);
                        }
                        else if (paragraphMatch.Success)
                        {
                            AppendParagraph(paragraphMatch, mainNode);
                        }
                        else if (imageMatch.Success)
                        {
                            AppendImage(mainNode, mainLineIndex, lines, 2, imageMatch);
                        }
                        else if (mainEndMatch.Success)
                        {
                            //Console.WriteLine($"Match com regex de fim do corpo principal na linha {mainLineIndex + 1}: {mainLine}");
                            break;
                        }
                    }
                }
                else if (footerMatch.Success)
                {
                    var footerId = footerMatch.Groups[1].Value.Trim();
                    var footerNode = HtmlNode.CreateNode($"<footer{(string.IsNullOrEmpty(footerId) ? "" : " id=" + footerId)}></footer>");
                    bodyNode.AppendChild(footerNode);

                    for (var footerLineIndex = lineIndex; footerLineIndex < lines.Length; footerLineIndex++)
                    {
                        var footerLine = lines[footerLineIndex];
                        var footerEndMatch = Regex.Match(footerLine, @"^\S");

                        var paragraphMatch = Regex.Match(footerLine, @"^\s\sparagrafo(.*):(.*)");

                        //Console.WriteLine("footerLine: " + footerLine);

                        if (paragraphMatch.Success)
                        {
                            var paragraph = paragraphMatch.Groups[2].Value.Trim();
                            paragraph = paragraph.EndsWith(";") ? paragraph.Substring(0, paragraph.Length - 1) : paragraph;
                            var paragraphNode = HtmlNode.CreateNode($"<p>{paragraph}</p>");
                            footerNode.AppendChild(paragraphNode);
                        }
                        else if (footerEndMatch.Success)
                        {
                            //Console.WriteLine($"Match com regex de fim do rodapé na linha {footerLineIndex + 1}: {footerLine}");
                            break;
                        }
                    }
                }
                lineIndex++;
            }

            static void RemoveQuotes(ref string str)
            {
                if (str.StartsWith("\"") && str.EndsWith("\""))
                {
                    str = str.Substring(1, str.Length - 2);
                }
            }

            static void AppendImage(HtmlNode node, int lineIndex, string[] lines, int identationLevel, Match? imageMatch = null)
            {
                HtmlNode imageNode;
                if (imageMatch != null)
                {
                    var imgId = imageMatch.Groups[1].Value.Trim();
                    //Console.WriteLine(imgId);
                    imageNode = HtmlNode.CreateNode($"<img {(string.IsNullOrEmpty(imgId) ? "" : "id=" + imgId)} />");
                }
                else
                {
                    imageNode = HtmlNode.CreateNode("<img />");
                }

                node.AppendChild(imageNode);

                for (var imageLineIndex = lineIndex + 1; imageLineIndex < lines.Length; imageLineIndex++)
                {
                    var imageLine = lines[imageLineIndex];

                    //Console.WriteLine("imageLine: " + imageLine);

                    var pathRegex = GetRegexByIdentationLevel(identationLevel, "caminho");

                    var pathMatch = Regex.Match(imageLine, pathRegex);
                    var altMatch = Regex.Match(imageLine, @"^\s\s\s\stexto alternativo(.*):(.*)");

                    if (pathMatch.Success)
                    {
                        var input = pathMatch.Groups[0].Value.Trim();

                        var base64Match = Regex.Match(input, @"data:image");

                        var httpMatch = Regex.Match(input, @"caminho\s*:\s*(http.*)");

                        if (base64Match.Success)
                        {
                            var base64PathMatch = Regex.Match(input, @"caminho(\s*):(.*)");
                            var base64Path = base64PathMatch.Groups[2].Value.Trim();

                            imageNode.Attributes.Add("src", base64Path);
                        }
                        else if (httpMatch.Success)
                        {
                            imageNode.Attributes.Add("src", httpMatch.Groups[1].Value.Trim());
                        }
                        else
                        {
                            var imgPath = pathMatch.Groups[2].Value.Trim();
                            imgPath = imgPath.EndsWith(";") ? imgPath.Substring(0, imgPath.Length - 1) : imgPath;
                            imgPath = imgPath.Replace(@"\", "/");
                            imageNode.Attributes.Add("src", imgPath);
                        }
                    }
                    else if (altMatch.Success)
                    {
                        var alt = altMatch.Groups[2].Value.Trim();
                        alt = alt.EndsWith(";") ? alt.Substring(0, alt.Length - 1) : alt;
                        imageNode.Attributes.Add("alt", alt);
                    }
                    else
                    {
                        //Console.WriteLine($"entrou aqui na linha {imageLineIndex + 1}: {imageLine}");
                        break;
                    }
                }
            }

            static void AppendButton(HtmlNode node, int lineIndex, string[] lines, int identationLevel, Match buttonMatch)
            {
                var buttonId = buttonMatch.Groups[1].Value.Trim();
                var buttonNode = HtmlNode.CreateNode($"<button {(string.IsNullOrEmpty(buttonId) ? "" : "id=" + buttonId)}></button>");
                buttonNode.AddClass("btn");
                node.AppendChild(buttonNode);

                for (var buttonLineIndex = lineIndex + 1; buttonLineIndex < lines.Length; buttonLineIndex++)
                {
                    var buttonLine = lines[buttonLineIndex];

                    //Console.WriteLine("buttonLine: " + buttonLine);

                    var textRegex = GetRegexByIdentationLevel(identationLevel, "texto");
                    var outlineRegex = GetRegexByIdentationLevel(identationLevel, "contorno");

                    var textMatch = Regex.Match(buttonLine, textRegex);
                    var outlineMatch = Regex.Match(buttonLine, outlineRegex);

                    if (textMatch.Success)
                    {
                        var text = textMatch.Groups[2].Value.Trim();
                        text = text.EndsWith(";") ? text.Substring(0, text.Length - 1) : text;
                        buttonNode.InnerHtml = text;
                    }
                    else if (outlineMatch.Success)
                    {
                        var outline = outlineMatch.Groups[2].Value.Trim();
                        outline = outline.EndsWith(";") ? outline.Substring(0, outline.Length - 1) : outline;
                        var outlineClass = outline switch
                        {
                            "linha" => "btn-outline",
                            "preencher" => "btn-filled",
                            "nenhum" => "btn-none",
                            _ => ""
                        };

                        buttonNode.AddClass(outlineClass);
                    }
                    else
                    {
                        //Console.WriteLine($"entrou aqui na linha {buttonLineIndex + 1}: {buttonLine}");
                        break;
                    }
                }
            }

            static void AppendSearchBar(HtmlNode node, int lineIndex, string[] lines, int identationLevel, Match searchBarMatch)
            {
                var searchBarId = searchBarMatch.Groups[1].Value.Trim();
                var inputNode = HtmlNode.CreateNode($"<input type=\"text\" {(string.IsNullOrEmpty(searchBarId) ? "" : "id=" + searchBarId)} />");
                node.AppendChild(inputNode);

                for (var searchBarLineIndex = lineIndex + 1; searchBarLineIndex < lines.Length; searchBarLineIndex++)
                {
                    var searchBarLine = lines[searchBarLineIndex];

                    //Console.WriteLine("searchBarLine: " + searchBarLine);

                    var textRegex = GetRegexByIdentationLevel(identationLevel, "texto");
                    var borderRegex = GetRegexByIdentationLevel(identationLevel, "borda");

                    var textMatch = Regex.Match(searchBarLine, textRegex);
                    var borderMatch = Regex.Match(searchBarLine, borderRegex);

                    if (textMatch.Success)
                    {
                        var text = textMatch.Groups[2].Value.Trim();
                        text = text.EndsWith(";") ? text.Substring(0, text.Length - 1) : text;
                        inputNode.Attributes.Add("placeholder", text);
                    }
                    else if (borderMatch.Success)
                    {
                        var border = borderMatch.Groups[2].Value.Trim();
                        border = border.EndsWith(";") ? border.Substring(0, border.Length - 1) : border;
                    }
                    else
                    {
                        //Console.WriteLine($"entrou aqui na linha {searchBarLineIndex + 1}: {searchBarLine}");
                        break;
                    }
                }
            }

            static void AppendGroup(HtmlNode node, int lineIndex, string[] lines, int identationLevel, Match groupMatch)
            {
                var groupId = groupMatch.Groups[1].Value.Trim();
                var groupNode = HtmlNode.CreateNode($"<div {(string.IsNullOrEmpty(groupId) ? "" : "id=" + groupId)}></div>");
                node.AppendChild(groupNode);

                for (var groupLineIndex = lineIndex + 1; groupLineIndex < lines.Length; groupLineIndex++)
                {
                    var groupLine = lines[groupLineIndex];

                    //Console.WriteLine("groupLine: " + groupLine);

                    var imageRegex = GetRegexByIdentationLevel(identationLevel, "imagem");
                    var searchRegex = GetRegexByIdentationLevel(identationLevel, "busca");
                    var buttonRegex = GetRegexByIdentationLevel(identationLevel, "bot[ãa]o");
                    var titleRgex = GetRegexByIdentationLevel(identationLevel, "t[ií]tulo");
                    var subtitleRegex = GetRegexByIdentationLevel(identationLevel, "subt[íi]tulo");
                    var paragraphRegex = GetRegexByIdentationLevel(identationLevel, "paragrafo");
                    var sectionRegex = GetRegexByIdentationLevel(identationLevel, "se[çc][ãa]o");
                    var articleRegex = GetRegexByIdentationLevel(identationLevel, "artigo");

                    var imageMatch = Regex.Match(groupLine, imageRegex);
                    var searchMatch = Regex.Match(groupLine, searchRegex);
                    var buttonMatch = Regex.Match(groupLine, buttonRegex);
                    var titleMatch = Regex.Match(groupLine, titleRgex);
                    var subtitleMatch = Regex.Match(groupLine, subtitleRegex);
                    var paragraphMatch = Regex.Match(groupLine, paragraphRegex);
                    var sectionMatch = Regex.Match(groupLine, sectionRegex);
                    var articleMatch = Regex.Match(groupLine, articleRegex);
                    var groupEndMatch = Regex.Match(groupLine, GetEndRegexByIdentationLevel(identationLevel));

                    if (imageMatch.Success)
                    {
                        AppendImage(groupNode, groupLineIndex, lines, identationLevel + 1, imageMatch);
                    }
                    else if (searchMatch.Success)
                    {
                        AppendSearchBar(groupNode, groupLineIndex, lines, identationLevel + 1, searchMatch);
                    }
                    else if (buttonMatch.Success)
                    {
                        AppendButton(groupNode, groupLineIndex, lines, identationLevel + 1, buttonMatch);
                    }
                    else if (titleMatch.Success)
                    {
                        AppendTittle(titleMatch, groupNode);
                    }
                    else if (subtitleMatch.Success)
                    {
                        AppendSubtitle(subtitleMatch, groupNode);
                    }
                    else if (paragraphMatch.Success)
                    {
                        AppendParagraph(paragraphMatch, groupNode);
                    }
                    else if (sectionMatch.Success)
                    {
                        AppendSection(groupNode, groupLineIndex, lines, identationLevel + 1, sectionMatch);
                    }
                    else if (articleMatch.Success)
                    {
                        AppendArticle(groupNode, groupLineIndex, lines, identationLevel + 1, articleMatch);
                    }
                    else if (groupEndMatch.Success)
                    {
                        //Console.WriteLine($"entrou aqui na linha {groupLineIndex + 1}: {groupLine}");
                        break;
                    }
                }
            }

            static void AppendNav(HtmlNode node, int lineIndex, string[] lines, int identationLevel, Match navMatch)
            {
                var navId = navMatch.Groups[1].Value.Trim();
                var navNode = HtmlNode.CreateNode($"<nav {(string.IsNullOrEmpty(navId) ? "" : "id=" + navId)}></nav>");
                node.AppendChild(navNode);
                var ulNode = HtmlNode.CreateNode("<ul></ul>");
                navNode.AppendChild(ulNode);

                for (var navLineIndex = lineIndex + 1; navLineIndex < lines.Length; navLineIndex++)
                {
                    var navLine = lines[navLineIndex];

                    //Console.WriteLine("navLine: " + navLine);

                    var optionMatch = Regex.Match(navLine, @"^\s{" + identationLevel * 2 + "}-(.*)");
                    var navEndMatch = Regex.Match(navLine, @"^(?!\s{" + identationLevel + @",}-\s).*$");

                    if (optionMatch.Success)
                    {
                        var option = optionMatch.Groups[1].Value.Trim();
                        var optionsArray = option.Split(":");
                        var aNodeContent = $"<a href=\"#\">{option}</a>";
                        if (optionsArray.Length > 1)
                        {
                            var aNodeId = optionsArray[1].Trim();
                            aNodeContent = $"<a href=\"#{aNodeId}\">{optionsArray[0]}</a>";
                        }
                        else
                        {
                            Console.WriteLine($"Aviso: Opção de navegação sem id na linha {navLineIndex + 1}");
                            Console.WriteLine($"{navLine}");
                        }
                        var aNode = HtmlNode.CreateNode(aNodeContent);
                        var liNode = HtmlNode.CreateNode($"<li></li>");
                        liNode.AppendChild(aNode);
                        ulNode.AppendChild(liNode);
                    }
                    else if (navEndMatch.Success)
                    {
                        //Console.WriteLine($"entrou aqui na linha {navLineIndex + 1}: {navLine}");
                        break;
                    }
                }
            }

            static void AppendTittle(Match titleMatch, HtmlNode mainNode, bool isMainTitle = false)
            {
                var title = titleMatch.Groups[2].Value.Trim();
                var titleId = titleMatch.Groups[1].Value.Trim();
                title = title.EndsWith(";") ? title.Substring(0, title.Length - 1) : title;
                var titleNode = HtmlNode.CreateNode($"<{(isMainTitle ? "h1" : "h2")} {(string.IsNullOrEmpty(titleId) ? "" : "id=" + titleId)}>{title}</{(isMainTitle ? "h1" : "h2")}>");
                mainNode.AppendChild(titleNode);
            }

            static void AppendSubtitle(Match subtitleMatch, HtmlNode mainNode)
            {
                var subtitle = subtitleMatch.Groups[2].Value.Trim();
                var subtitleId = subtitleMatch.Groups[1].Value.Trim();
                subtitle = subtitle.EndsWith(";") ? subtitle.Substring(0, subtitle.Length - 1) : subtitle;
                var subtitleNode = HtmlNode.CreateNode($"<h3{(string.IsNullOrEmpty(subtitleId) ? "" : " id=" + subtitleId)}>{subtitle}</h3>");
                mainNode.AppendChild(subtitleNode);
            }

            static void AppendParagraph(Match paragraphMatch, HtmlNode mainNode)
            {
                var paragraph = paragraphMatch.Groups[2].Value.Trim();
                var paragraphId = paragraphMatch.Groups[1].Value.Trim();
                paragraph = paragraph.EndsWith(";") ? paragraph.Substring(0, paragraph.Length - 1) : paragraph;
                var paragraphNode = HtmlNode.CreateNode($"<p{(string.IsNullOrEmpty(paragraphId) ? "" : " id=" + paragraphId)}>{paragraph}</p>");
                mainNode.AppendChild(paragraphNode);
            }

            static void AppendSection(HtmlNode node, int lineIndex, string[] lines, int identationLevel, Match sectionMatch)
            {
                var sectionId = sectionMatch.Groups[1].Value.Trim();
                var sectionNode = HtmlNode.CreateNode($"<section {(string.IsNullOrEmpty(sectionId) ? "" : "id = \"" + sectionId + "\"")}></section>");
                node.AppendChild(sectionNode);

                for (var sectionLineIndex = lineIndex + 1; sectionLineIndex < lines.Length; sectionLineIndex++)
                {
                    var sectionLine = lines[sectionLineIndex];

                    //Console.WriteLine("sectionLine: " + sectionLine);

                    var titleRegex = GetRegexByIdentationLevel(identationLevel, @"t[ií]tulo");
                    var subtitleRegex = GetRegexByIdentationLevel(identationLevel, @"subt[íi]tulo");
                    var paragraphRegex = GetRegexByIdentationLevel(identationLevel, @"paragrafo");
                    var imageRegex = GetRegexByIdentationLevel(identationLevel, @"imagem");

                    var titleMatch = Regex.Match(sectionLine, titleRegex);
                    var subtitleMatch = Regex.Match(sectionLine, subtitleRegex);
                    var paragraphMatch = Regex.Match(sectionLine, paragraphRegex);
                    var imageMatch = Regex.Match(sectionLine, imageRegex);
                    var sectionEndMatch = Regex.Match(sectionLine, GetEndRegexByIdentationLevel(identationLevel));

                    if (titleMatch.Success)
                    {
                        AppendTittle(titleMatch, sectionNode);
                    }
                    else if (subtitleMatch.Success)
                    {
                        AppendSubtitle(subtitleMatch, sectionNode);
                    }
                    else if (paragraphMatch.Success)
                    {
                        AppendParagraph(paragraphMatch, sectionNode);
                    }
                    else if (imageMatch.Success)
                    {
                        AppendImage(sectionNode, sectionLineIndex, lines, identationLevel + 1, imageMatch);
                    }
                    else if (sectionEndMatch.Success)
                    {
                        //Console.WriteLine($"entrou aqui na linha {sectionLineIndex + 1}: {sectionLine}");
                        break;
                    }
                }
            }

            static void AppendArticle(HtmlNode node, int lineIndex, string[] lines, int identationLevel, Match articleMatch)
            {
                var articleId = articleMatch.Groups[1].Value.Trim();
                var articleNode = HtmlNode.CreateNode($"<article {(string.IsNullOrEmpty(articleId) ? "" : "id = \"" + articleId + "\"")}></article>");
                node.AppendChild(articleNode);

                for (var articleLineIndex = lineIndex + 1; articleLineIndex < lines.Length; articleLineIndex++)
                {
                    var articleLine = lines[articleLineIndex];

                    //Console.WriteLine("articleLine: " + articleLine);

                    var sectionRegex = GetRegexByIdentationLevel(identationLevel, @"se[çc][ãa]o");
                    var titleRegex = GetRegexByIdentationLevel(identationLevel, @"t[ií]tulo");
                    var subtitleRegex = GetRegexByIdentationLevel(identationLevel, @"subt[íi]tulo");
                    var paragraphRegex = GetRegexByIdentationLevel(identationLevel, @"paragrafo");
                    var imageRegex = GetRegexByIdentationLevel(identationLevel, @"imagem");

                    var sectionMatch = Regex.Match(articleLine, sectionRegex);
                    var titleMatch = Regex.Match(articleLine, titleRegex);
                    var subtitleMatch = Regex.Match(articleLine, subtitleRegex);
                    var paragraphMatch = Regex.Match(articleLine, paragraphRegex);
                    var imageMatch = Regex.Match(articleLine, imageRegex);
                    var articleEndMatch = Regex.Match(articleLine, GetEndRegexByIdentationLevel(identationLevel));

                    if (sectionMatch.Success)
                    {
                        AppendSection(articleNode, articleLineIndex, lines, identationLevel + 1, sectionMatch);
                    }
                    else if (titleMatch.Success)
                    {
                        AppendTittle(titleMatch, articleNode);
                    }
                    else if (subtitleMatch.Success)
                    {
                        AppendSubtitle(subtitleMatch, articleNode);
                    }
                    else if (paragraphMatch.Success)
                    {
                        AppendParagraph(paragraphMatch, articleNode);
                    }
                    else if (imageMatch.Success)
                    {
                        AppendImage(articleNode, articleLineIndex, lines, identationLevel + 1, imageMatch);
                    }
                    else if (articleEndMatch.Success)
                    {
                        //Console.WriteLine($"entrou aqui na linha {articleLineIndex + 1}: {articleLine}");
                        break;
                    }
                }
            }

            static string GetEndRegexByIdentationLevel(int identationLevel, bool isList = false)
            {
                var regex = @"^(?!\s{" + identationLevel * 2 + @"})";
                if (isList)
                {
                    regex += @"(?!\s{" + (identationLevel * 2 + 2) + @"}-\s)";
                }
                regex += @".*$";
                return regex;
            }

            static string GetRegexByIdentationLevel(int identationLevel, string mainRegex)
            {
                var regex = @"^\s{" + identationLevel * 2 + @"}";
                regex += mainRegex + @"(.*):(.*)";
                //Console.WriteLine("Regex: " + regex);
                return regex;
            }

            return htmlDoc.DocumentNode.OuterHtml;
        }
    }
}



/* input yaml format example:
configurações:
  idioma: "pt"
  título: Minha primeira página web
  tema: "padrão-escuro"

cabeçalho:
  posição: superior
  imagem 1:
    caminho: Logo.png
    texto alternativo: Texto alternativo da imagem do header
  navegação:
    - Início
    - Quem Somos
    - Trabalhe Conosco
    - Fale conosco
  grupo 1:
    busca:
      texto: Digite o que procura
      borda: linha
    botão 1:
      texto: Log in
      contorno: linha
    botão 2:
      texto: Cadastro
      contorno: preencher

corpo principal:
  título: Título da página
  artigo:
    seção 1:
      título: Título da primeira seção
      subtítulo: Subtítulo da primeira seção
      paragrafo: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas facilisis convallis mi, vitae dictum nisl euismod in. Suspendisse consequat, dui eget consequat tempus, nunc turpis fermentum nisl, nec lacinia tortor elit luctus erat. Donec ullamcorper eget elit eget luctus. Suspendisse blandit blandit felis quis tincidunt.
    seção 2:
      título: Título da segunda seção
      subtítulo: Subtítulo da segunda seção
      paragrafo: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas facilisis convallis mi, vitae dictum nisl euismod in. Suspendisse consequat, dui eget consequat tempus, nunc turpis fermentum nisl, nec lacinia tortor elit luctus erat. Donec ullamcorper eget elit eget luctus. Suspendisse blandit blandit felis quis tincidunt.
      imagem:
        caminho: Paisagem.jpg
        texto alternativo: Texto alternativo da imagem do body

rodapé:
  paragrafo: Todos os direitos reservados
*/

/* output html format example:
 <!DOCTYPE html>
<html lang="pt">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Minha primeira página web</title>
    <style>
        /* Estilos básicos * /
        body {
            font-family: Arial, sans-serif;
            margin: 0;
        }

        header {
            position: sticky;
            top: 0;
            background-color: #f4f4f4;
            padding: 10px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        header img {
            max-width: 120px;
        }

        nav ul {
            list-style-type: none;
            padding: 0;
        }

        nav ul li {
            display: inline;
            margin-right: 20px;
        }

        .search-bar {
            border: 1px solid black;
            padding: 5px;
        }

        .btn {
            cursor: pointer;
            padding: 10px 15px;
            margin-left: 10px;
        }

        .btn-outline {
            border: 1px solid black;
            background-color: transparent;
        }

        .btn-filled {
            background-color: black;
            color: white;
        }

        main {
            padding: 20px 20%;
        }

        footer {
            background-color: #f4f4f4;
            padding: 10px;
            text-align: center;
        }
    </style>
</head>

<body>

    <!-- Cabeçalho -->
    <header>
        <img src="Teste.jpg" alt="Texto alternativo da imagem do header">
        <nav>
            <ul>
                <li><a href="#">Início</a></li>
                <li><a href="#">Quem Somos</a></li>
                <li><a href="#">Trabalhe Conosco</a></li>
                <li><a href="#">Fale conosco</a></li>
            </ul>
        </nav>
        <div>
            <input type="text" class="search-bar" placeholder="Digite o que procura">
            <button class="btn btn-outline">Log in</button>
            <button class="btn btn-filled">Cadastro</button>
        </div>
    </header>

    <!-- Corpo principal -->
    <main>
        <article>
            <h2>Olá, mundo!</h2>
            <h3>bem vindo</h3>
            <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas facilisis convallis mi, vitae dictum
                nisl euismod in. Suspendisse consequat, dui eget consequat tempus, nunc turpis fermentum nisl, nec
                lacinia tortor elit luctus erat. Donec ullamcorper eget elit eget luctus. Suspendisse blandit blandit
                felis quis tincidunt.</p>
            <img src="Teste.jpg" alt="Texto alternativo da imagem do body">
        </article>
    </main>

    <!-- Rodapé -->
    <footer>
        <p>Todos os direitos reservados</p>
    </footer>

</body>

</html>


*/
