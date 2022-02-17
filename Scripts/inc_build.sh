
 #!/bin/bash
 
DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
cd "${DIR}"

cd ../TalkiPlay.iOS/Config/Scripts
./inc_build.sh

cd ../../../TalkiPlay.Android/Config
./Scripts/inc_build.py
