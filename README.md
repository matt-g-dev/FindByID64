# FindByID64


Small utility to grab JSON lines from bigger JSON dumps by ID64.


Command line options:

    -i, --input id64_list.txt                     * Import ID64s from id64_list.txt (1 per line)
    -q, --id64s id64_1,id64_2...                  * Specify comma-seperated ID64s on command line
    -j, --json filename.json(.gz)                 * Source JSON file to search (in plain text or GZIP format)
    -o, --output output.json                      * File to write matching lines to (default: output.json)
    -m, --multiple                                * Match multiple times (default: false)
    -k, --key [id64|SystemAddress|systemId64]     * JSON key to match against ID64 (default: id64)
    -v, --verbose                                 * Enable verbose output (default: false)
  
Example usage:

    FindByID64.exe -q 4374164130139,3932277478106 -j systemsWithCoordinates.json.gz -o best_systems.json

This will match ID64s 4374164130139 and 3932277478106 in systemsWithCoordinates.json.gz and write lines to best_systems.json



