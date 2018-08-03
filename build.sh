#!/bin/bash
#---
# Excerpted from "Domain Modeling Made Functional",
# published by The Pragmatic Bookshelf.
# Copyrights apply to this code. It may not be used to create training material,
# courses, books, articles, and the like. Contact us if you are in doubt.
# We make no guarantees that this code is fit for any purpose.
# Visit http://www.pragmaticprogrammer.com/titles/swdddf for more book information.
#---
if test "$OS" = "Windows_NT"
then
  # use .Net

  .paket/paket.exe restore
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
  	exit $exit_code
  fi

  packages/build/FAKE/tools/FAKE.exe $@ --fsiargs build.fsx
else
  # use mono
  mono .paket/paket.exe restore
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
  	exit $exit_code
  fi
  mono packages/build/FAKE/tools/FAKE.exe $@ --fsiargs -d:MONO build.fsx
fi
