#!/bin/bash

## THE FOLLOWING SOURCE CODE WAS TAKEN FROM
## SOURCE: https://gist.github.com/garethstephenson/cf76e71e48c2335ad22b918d7e8f5d3d

if [[ -z "$1" ]]
then
  echo "ERROR: You must specify either a -get, major, minor or patch argument, and a project file."
  echo "Usage: $0 (-get|major|minor|patch) <project-name>.csproj"
  exit 1
fi

# Define the project
PROJECT_PATH=Cassandra.Fluent.Migrator/Cassandra.Fluent.Migrator.csproj

CURRENT_VERSION=$(sed -n 's/.*<Version>\(.*\)<\/Version>.*/\1/p' $PROJECT_PATH)
if [ -z "$CURRENT_VERSION" ]
then
  echo "ERROR: Could not find a <Version/> tag in the project file '$PROJECT_PATH'."
  echo "Please add one in between the <Project><PropertyGroup> tags and try again."
  exit 3
fi

parts=(${CURRENT_VERSION//./ })
case "$1" in
  -get) echo $CURRENT_VERSION; exit 0 ;;
  major) ((parts[0]++)) ;;
  minor) ((parts[1]++)) ;;
  patch) ((parts[2]++)) ;;
  *)
    echo "ERROR: Invalid SemVer position name supplied, '$1' was not understood."
    echo "Usage: $0 (-get|major|minor|patch) <project-name>.csproj"
    exit 4
esac

NEW_VERSION="${parts[0]}.${parts[1]}.${parts[2]}"
$(sed -i -e "s/<Version>$CURRENT_VERSION<\/Version>/<Version>$NEW_VERSION<\/Version>/g" $PROJECT_PATH)
echo $NEW_VERSION