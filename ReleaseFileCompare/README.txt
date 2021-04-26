This tool is designed to identify which public methods changed between two branches. The primary purpose
is to identify any public function that changed as part of a release to help assist with release testing.

Before running this .NET program, we need to gather some initial information. In both cases, the branches
need to be modified from the scripts below, which all reference master and develop.

1) First, we need to get a list of all files that are changed as part of this change. The 200 is to help avoid truncating any
long file paths. The .cs with spaces is to (a) make sure we look at cs files and csproj files, and (b) to get rid of any (new) files


git diff master..develop --compact-summary --stat=200 | grep '.cs    ' | cut -d "|" -f 1 >> "fileList.txt"


2) Next, we need to download a copy of both the source and destination file from each of the branches. These will be 
saved into the workspace folder into an "a" and "b" folder which the .NET program will be used to gather.

git diff master..develop --compact-summary --stat=200 | grep '.cs    ' | cut -d "|" -f 1 | while read -r line; do 
dest="${line//\//_}"
echo $dest
git show master:$line >> CompareWorkspace\\a\\$dest 
git show develop:$line >> CompareWorkspace\\b\\$dest 
done


3) Finally, run the .NET program to use all these pieces to output a list of the public methods that were modified as part of this change
ReleaseFileCompare.exe "path-to-workspace-with-a-and-b-folders-and-fileList.txt"


