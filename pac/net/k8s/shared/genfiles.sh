export BIN_FOLDER=templates/.bin

mkdir -p ./$BIN_FOLDER

echo "Replacing Environment Variables in Template Files"
for file in $(find templates/*.yaml -type f)
do
    envsubst < $file > $BIN_FOLDER/${file##*/}
done