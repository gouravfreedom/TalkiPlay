
 #!/bin/bash
 
DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
cd "${DIR}"
cd ..

update_info_file () 
{
    plist_file="$1"
    buildNumber=$(/usr/libexec/PlistBuddy -c "Print CFBundleVersion" "$plist_file")
    buildNumber=$(($buildNumber + 1))
    /usr/libexec/PlistBuddy -c "Set :CFBundleVersion $buildNumber" "$plist_file"
}


update_info_file "Info_Dev.plist"
update_info_file "Info_Stg.plist"
update_info_file "Info_Prd.plist"



