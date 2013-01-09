#!/bin/sh

OLDDIR="$(pwd)"
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

echo "Entering $DIR"
cd $DIR

MONO="$(pidof mono)"
if [ $MONO ]
then
	echo "Stopping active searcher with pid ${MONO}"
	kill $MONO
else
	echo "Searcher not running"
fi

echo "Leaving $DIR"
cd $OLDDIR
