#!/usr/bin/python

import re

def update_file(filename):

    f = open(filename, 'r+')
    text = f.read()
    result = re.search(r'(?P<groupA>android:versionCode=")(?P<version>.*)(?P<groupB>")',text)

    version = str(int(result.group("version")) + 1)
    newVersionString = result.group("groupA") + version + result.group("groupB")
    newText = re.sub(r'android:versionCode=".*"', newVersionString, text);
    f.seek(0)
    f.write(newText)
    f.truncate()
    f.close()
    return


update_file("AndroidManifest_Dev.xml")
update_file("AndroidManifest_Stg.xml")
update_file("AndroidManifest_Prd.xml")


 