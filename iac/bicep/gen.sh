#!/bin/bash 
set -euo pipefail

bicep build rg.bicep
bicep build resources.bicep
bicep build permsaks.bicep