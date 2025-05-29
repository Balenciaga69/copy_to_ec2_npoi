#!/usr/bin/env python3
# -*- coding: utf-8 -*-
import os
import sys
sys.path.append('C:/Users/wits/Desktop/GitRepo/solo_everything/coding_playground/python/code_base_writer')
sys.path.append('C:/Custom/A-Code/solo_everything/coding_playground/python/code_base_writer')
from OutputFile import run_file_export

def main():
    targetPath = ""
    if os.getlogin() == "Gay":
        targetPath = "C:/Custom/A-Code/solo_everything/coding_playground/csharp/LabWorkTools/Modules/MyXSD"
    else:
        targetPath = "C:/Users/wits/Desktop/GitRepo/solo_everything/coding_playground/csharp/LabWorkTools/Modules/MyXSD"
    run_file_export(
        base_dir=f"{targetPath}",
        extensions=[".cs", ".js"],
        output_file="../xx_xsd_report.txt",
        exclude_patterns=["/obj", "/node_modules", "/migrations"],
        include_tree=True
    )
    # input("Press Enter to exit...")


if __name__ == "__main__":
    main()
