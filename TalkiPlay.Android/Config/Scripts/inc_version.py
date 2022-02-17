#!/usr/bin/python

import re

def update_file(filename):

    f = open(filename, 'r+')
    text = f.read()
    result = re.search(r'(?P<groupA>android:versionName=")(?P<version>.*)(?P<groupB>")',text)
    
    versionNumbers = result.group("version").split('.')
    
    versionNumbers[-1] = unicode(int(versionNumbers[-1]) + 1)
    newVersion = ".".join(versionNumbers).encode('utf-8')        
    newVersionString = result.group("groupA") + newVersion + result.group("groupB")    
    newText = re.sub(r'android:versionName=".*"', newVersionString, text);
    f.seek(0)
    f.write(newText)
    f.truncate()
    f.close()
    return

update_file("AndroidManifest_Dev.xml")
update_file("AndroidManifest_Stg.xml")
update_file("AndroidManifest_Prd.xml")



 