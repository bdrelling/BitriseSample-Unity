#!/usr/bin/env bash

sudo -S installer -package ./unity.pkg -target / -verbose
sudo -S installer -package ./ios.pkg -target / -verbose
#sudo -S installer -package ./android.pkg -target / -verbose