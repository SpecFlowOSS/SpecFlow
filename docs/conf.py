import sys, os
# Configuration file for the Sphinx documentation builder.
#
# This file only contains a selection of the most common options. For a full
# list see the documentation:
# https://www.sphinx-doc.org/en/master/usage/configuration.html

# -- Path setup --------------------------------------------------------------

# If extensions (or modules to document with autodoc) are in another directory,
# add these directories to sys.path here. If the directory is relative to the
# documentation root, use os.path.abspath to make it absolute, like shown here.
#
# import os
# import sys
# sys.path.insert(0, os.path.abspath('.'))


# -- Project information -----------------------------------------------------

project = ''
copyright = '2020, The SpecFlow Team'
author = 'The SpecFlow Team'


# -- General configuration ---------------------------------------------------

# Add any Sphinx extension module names here, as strings. They can be
# extensions coming with Sphinx (named 'sphinx.ext.*') or your custom
# ones.
extensions = ['recommonmark',  "sphinx_rtd_theme",  "sphinx_markdown_tables", "sphinx_search.extension", "sphinx_sitemap_dev"]

# Add any paths that contain templates here, relative to this directory.
templates_path = ['_templates']

# List of patterns, relative to source directory, that match files and
# directories to ignore when looking for source files.
# This pattern also affects html_static_path and html_extra_path.
exclude_patterns = ['_build', 'Thumbs.db', '.DS_Store']

master_doc = 'index'
# -- Options for HTML output -------------------------------------------------

# The theme to use for HTML and HTML Help pages.  See the documentation for
# a list of builtin themes.
#
html_theme = 'sphinx_rtd_theme'

# Add any paths that contain custom static files (such as style sheets) here,
# relative to this directory. They are copied after the builtin static files,
# so a file named "default.css" will overwrite the builtin "default.css".
html_static_path = ['_static']
html_theme_options = {
    'logo_only': True,
    'style_nav_header_background': '#e9e7ee',
    'analytics_id':'UA-11088967-5'
}
html_logo = '_static/logo.png'
html_css_files = [
    'css/custom.css'
]

html_js_files = [
    'js/hotjar.js',
]

html_baseurl = 'https://docs.specflow.org/projects/specflow/'
html_extra_path = ['robots.txt']

sys.path.append(os.path.abspath('exts'))

sitemap_filename = 'sitemap_generated.xml'
sitemap_url_scheme = "{lang}latest/{link}"
