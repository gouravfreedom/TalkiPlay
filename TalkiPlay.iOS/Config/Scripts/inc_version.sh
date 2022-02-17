
 #!/bin/bash
 
DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
cd "${DIR}"
cd ..

update_info_file () 
{
    plist_file="$1"
    oldVersion=$(/usr/libexec/PlistBuddy -c "Print CFBundleShortVersionString" "$plist_file")

    lowestVersion=`echo $oldVersion | awk -F "." '{print $3}'`
    lowestVersion=$(($lowestVersion + 1))
    newVersion=`echo $oldVersion | awk -F "." '{print $1 "." $2 ".'$lowestVersion'" }'`
    
    /usr/libexec/PlistBuddy -c "Set :CFBundleShortVersionString $newVersion" "$plist_file"
}

update_info_file "Info_Dev.plist"
update_info_file "Info_Stg.plist"
update_info_file "Info_Prd.plist"
