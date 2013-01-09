#!/bin/sh

monopath=/usr/local/bin/mono

OLDDIR="$(pwd)"
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

echo "Entering $DIR"
cd $DIR

binpath=Searcher.exe
logpath=console.log

MONO="$(pidof mono)"
if [ $MONO ]
then
	echo "Stopping active searcher with pid ${MONO}"
	kill $MONO
fi

echo "Starting searcher"
nohup $monopath $binpath cityfiles gvnj58 quiet >$logpath 2>&1 &

echo "Leaving $DIR"
cd $OLDDIR
