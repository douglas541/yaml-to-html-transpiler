import os
import re
from bs4 import BeautifulSoup

path = r"C:\Users\dougl\Documents\Dev\C#\HtmlCompiler\Files\input.txt"
basePath = r"C:\Users\dougl\Documents\Dev\C#\HtmlCompiler"

# Check file existence
if not os.path.exists(path):
    print(f"Arquivo {os.path.basename(path)} não encontrado!")
    exit()

# Create base HTML structure
soup = BeautifulSoup("<!DOCTYPE html><html lang='pt-br'></html>", 'html.parser')
html_node = soup.html
head_node = soup.new_tag("head")
meta_charset = soup.new_tag("meta", charset="UTF-8")
meta_viewport = soup.new_tag("meta", name="viewport", content="width=device-width, initial-scale=1.0")
head_node.append(meta_charset)
head_node.append(meta_viewport)
body_node = soup.new_tag("body")
html_node.append(head_node)
html_node.append(body_node)

# Helper functions
def process_line(line, pattern, node, action, substr_start):
    match = re.search(pattern, line)
    if match:
        action(node, line[substr_start:].strip().rstrip(';'))
        return True
    return False

def set_value_as_title(node, value):
    title_tag = soup.new_tag("title")
    title_tag.string = value
    node.append(title_tag)

def set_language_attribute(node, value):
    node['lang'] = value

# Process input file
with open(path, 'r', encoding='utf-8') as f:
    lines = f.readlines()

for line in lines:
    config_match = re.search(r"configura[çc][õoã](o|es)", line)
    header_match = re.search(r"cabe[çc]alho", line)
    main_match = re.search(r"corpo", line)
    footer_match = re.search(r"rodap[ée]", line)

    if config_match:
        # Process the configuration block
        for config_line in lines:
            if process_line(config_line, r"t[ií]tulo:", head_node, set_value_as_title, 8):
                continue
            if process_line(config_line, r"idioma:", html_node, set_language_attribute, 8):
                continue

            if config_line.strip().endswith(';'):
                break

    # Similar blocks for header, main, and footer can be added here as per your C# code.
    # The process would involve checking for specific patterns and manipulating the HTML structure accordingly.

# Print final HTML
print(soup.prettify())
