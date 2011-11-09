Feature: Multiline step arguments

Scenario: a scenario with a multiline argument
 Given there is a multiline text argument
  """
  Some Title, Eh?
  ==============
  Here is the first paragraph of my blog post. Lorem ipsum dolor sit amet,
  consectetur adipiscing elit.
  """
 When there is a multiline text argument
  """
  Some Title, Eh?
  ==============
  Here is the first paragraph of my blog post. Lorem ipsum dolor sit amet,
  consectetur adipiscing elit.
  """
 Then there is a multiline text argument
  """
  Some Title, Eh?
  ==============
  Here is the first paragraph of my blog post. Lorem ipsum dolor sit amet,
  consectetur adipiscing elit.
  """
