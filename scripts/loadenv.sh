echo "Loading Env Vars from ${DOTENV}"
set -o allexport; source $DOTENV; set +o allexport