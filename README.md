# Nop.Plugin.Misc.Goftino
NopCommerce Plugin for Goftino

Goftino plugin allow for integration of nop commerce and https://www.goftino.com/

This plugin was test on NopCommerce Version 4.60.3

Installation

1) Build the code 
2) Add output folder to \Nop.Web\Plugins
3) Restart IIS application pool for the application to discover the new plugin
4) Get the API get from goftino.com
5) Under Admin->Configuration->Local Plugins->Goftino Online Chat->Configuration
6) From the configuration Section 

Add the following for Goftino Code Template

<!---start GOFTINO code--->
<script type=""text/javascript"">
!function(){var i=""{GoftinoCode}"",a=window,d=document;function g(){var g=d.createElement(""script""),s="">
</script>
<!---end GOFTINO code--->

And add the Goftino Code obtained from the website to the "Goftino Code" textbox
