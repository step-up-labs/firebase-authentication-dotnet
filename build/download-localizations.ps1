function Convert-Everything([string] $rootDir, [string[]]$languages) {
  $neutralStrings = "$rootDir/en.xml"

  Write-DesignerFile $neutralStrings | Out-File -FilePath '../src/Auth.UI/Resources/AppResources.Designer.cs'
  Convert-StringsToResX $neutralStrings | Out-File -FilePath '../src/Auth.UI/Resources/AppResources.resx' -encoding utf8
    
  $languages | ForEach-Object { 
    $lng = $_
    Convert-StringsToResX "$rootDir/$lng.xml" | Out-File -FilePath "../src/Auth.UI/Resources/AppResources.$lng.resx" -encoding utf8
  }
}

function Convert-StringsToResX([string] $strings) {
  Write-ResXHeader

  # android strings
  [string]$xmlString = Get-Content -Path $strings -Encoding UTF8
  [xml]$xml = $xmlString.Replace('&lt;! [CDATA [', '<![CDATA[').Replace('&lt;![CDATA [', '<![CDATA[')
  $nodes = $xml.SelectNodes("/resources/string")
  foreach ($node in $nodes) {
    #Write-Output $("{0,-50} : $($node.InnerText)" -f $node.name)
    $value = $node.InnerText

    if ($useMap) {
      if ($map.Keys -contains $node.name) {
        $value = $value.Replace('%s', '').Replace('%1$s', '').Replace('%2$s', '').Replace('%3$s', '').Replace('%4$s', '').Replace(':', '').Replace('<b>', '').Replace('</b>', '').Replace('<![CDATA[', '').Replace(']]>', '')
        $value = $value.Trim()
        $value = $value.Substring(0, 1).ToUpper() + $value.Substring(1, $value.Length - 1)
        Write-ResXItem $map[$node.name] $value
      }
    }
    else {
      Write-ResXItem $node.name $value
    }
  }

  # android strings plurals
  $nodes = $xml.SelectNodes("/resources/plurals")
  foreach ($node in $nodes) {
    #Write-Output $("{0,-50} : $($node.InnerText)" -f $node.name)
    foreach ($item in $node.SelectNodes("item")) { 
      $value = $item.InnerText

      if (-Not($useMap)) {
        Write-ResXItem "$($node.name)_$($item.quantity)" $value
      }
    }
  }

  Write-ResXFooter
}

function Write-ResXItem([string] $name, [string] $value) {
  $text = $value -replace "â€¦", "…" -replace "&amp;", "&" -replace "\\'", "'" -replace "%%", "%" -replace 'â€™', "'" -replace 'â€“', '-' -replace '%s', '{0}' -replace '%d', '{0}' -replace '%1\$s', '{0}' -replace '%2\$s', '{1}' -replace '%3\$s', '{2}' -replace '%4\$s', '{3}' -replace '%1\$d', '{0}' -replace '%2\$d', '{1}' -replace '\\n', "`n"
  $text = $text -replace "Google Play", 'Microsoft Store' -replace "v Obchodu Play", 'v Microsoft Store'
	
  $startData = ''
  $endData = ''
  if (-Not($text.StartsWith('<![CDATA['))) {
    $startData = '<![CDATA['
    $endData = ']]>'
  }
  Write-Output "  <data name=`"$name`" xml:space=`"preserve`">
    <value>$startData$text$endData</value>
  </data>"
}

function Write-ResXHeader {
  Write-Output '<?xml version="1.0" encoding="utf-8"?>
<root>
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
'
}

function Write-ResXFooter {
  Write-Output '
</root>'
} 

function Write-DesignerFile([string] $neutralResource) {
  Write-DesignerHeader

  # strings
  [xml]$neutralXml = Get-Content -Path $neutralResource
  $nodes = $neutralXml.SelectNodes("/resources/string")
  foreach ($node in $nodes) {
    Write-DesignerProperty $node.name $node.InnerText
  }

  # plurals
  $nodes = $neutralXml.SelectNodes("/resources/plurals")
  foreach ($node in $nodes) {
    foreach ($item in $node.SelectNodes("item")) { 
      Write-DesignerProperty "$($node.name)_$($item.quantity)" $item.InnerText
    }

    Write-PluralMethod $node.name
  }

  Write-DesignerFooter

}

function Write-PluralMethod([string] $name) {
  $camel = ($name -csplit '_' | foreach { (Get-Culture).TextInfo.ToTitleCase($_) }) -join ''
  Write-Output "        public string $($camel)WithCount(int count) {
            var key = `"`";
			switch (count)
            {
                case 1: 
					key = `"$($name)_one`"; 
					break;
                case var c when (c < 5):
					key = `"$($name)_few`"; 
					break;
                default: 
					key = `"$($name)_other`"; 
					break;;
            };
			var x = ResourceManager.GetString(key, this.resourceCulture);
			return string.IsNullOrWhiteSpace(x) ? @`"$name`" : string.Format(x, count);
        }
"
}

function Write-DesignerProperty([string] $name, [string] $value) {
  $camel = ($name -csplit '_' | foreach { (Get-Culture).TextInfo.ToTitleCase($_) }) -join ''
  $val = $value -replace '\\"', '""' -replace "`n", " " -replace "`r", " "
  $comment = $val -replace '&', '&amp;' 
  Write-Output "        /// <summary>
        /// $comment
        /// </summary>
		public string $camel {
            get {
                var x = ResourceManager.GetString(`"$name`", this.resourceCulture);
				return string.IsNullOrWhiteSpace(x) ? @`"$val`" : x;
            }
        }
"
}

function Write-DesignerHeader {
  Write-Output '//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Firebase.Auth.UI.Resources 
{
    using System;
	
	/// <summary>
    /// A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class AppResources {
        
        private global::System.Resources.ResourceManager resourceMan;
        
        private global::System.Globalization.CultureInfo resourceCulture;
        private static AppResources instance;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public AppResources() {
			instance = this;
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Firebase.Auth.UI.Resources.AppResources", typeof(AppResources).Assembly);
                    this.resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current threads CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public global::System.Globalization.CultureInfo Culture {
            get {
                return this.resourceCulture;
            }
            set {
                this.resourceCulture = value;
            }
        }

		public static AppResources Instance {
			get {
				return instance ?? (instance = new AppResources());
			}
		}
'
}

function Write-DesignerFooter {
  Write-Output '    }
}'
}

function Download-Localizations([string] $folder, [string[]]$languages) {
  if (-not(Test-Path $folder)) {
    mkdir $folder
  }

  Write-Output "Starting"

  # English
  Invoke-WebRequest -uri "https://raw.githubusercontent.com/firebase/FirebaseUI-Android/master/auth/src/main/res/values/strings.xml" -OutFile "$folder/en.xml"

  $languages | foreach {

    $lng = $_
    try {
      Write-Output "Downloading $lng"
             
      # Firebase Auth
      Invoke-WebRequest -uri "https://raw.githubusercontent.com/firebase/FirebaseUI-Android/master/auth/src/main/res/values-$lng/strings.xml" -OutFile "$folder/$lng.xml"
    }
    catch {
      Write-Error "Couldn't fetch language file '$lng'"
    }
  }
}

[string[]]$languages = 
"ar",
"bg",
"ca",
"zh",
"hr",
"cs",
"da",
"nl",
"fa",
"fil",
"fi",
"fr",
"de",
"el",
"es",
"iw",
"hi",
"hu",
"in",
"it",
"ja",
"ko",
"lv",
"lt",
"no",
"pl",
"pt",
"ro",
"ru",
"sr",
"sk",
"sl",
"sv",
"th",
"tr",
"uk",
"ur",
"vi"


$folder = "temp"

Write-Output "Download files from github"
Download-Localizations $folder $languages

Write-Output "Generating ResX"
Convert-Everything $folder $languages

Write-Output "Cleanup"
Remove-Item $folder -Force -Recurse


