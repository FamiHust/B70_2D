import os
import re

workspace_root = r"c:\B70_2D"

# Search patterns
patterns = {
    "ItemData": r"class\s+ItemData\b",
    "ItemsCollection": r"class\s+ItemsCollection\b",
    "max_level": r"(max\s*level|level\s*limit|MAX_LEVEL|MaxLevel)",
}

results = {
    "ItemData": [],
    "ItemsCollection": [],
    "max_level": [],
}

# Search through all C# files
for root, dirs, files in os.walk(workspace_root):
    # Skip common unneeded directories
    dirs[:] = [d for d in dirs if d not in ['.git', 'node_modules', '.vs', 'Library', 'Temp', 'obj', 'bin']]
    
    for file in files:
        if file.endswith('.cs'):
            filepath = os.path.join(root, file)
            try:
                with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
                    content = f.read()
                    
                    if re.search(patterns["ItemData"], content):
                        results["ItemData"].append(filepath)
                    
                    if re.search(patterns["ItemsCollection"], content):
                        results["ItemsCollection"].append(filepath)
                    
                    if re.search(patterns["max_level"], content, re.IGNORECASE):
                        results["max_level"].append(filepath)
            except Exception as e:
                pass

print("=" * 80)
print("SEARCH RESULTS")
print("=" * 80)

print("\n1. ItemData class definition:")
if results["ItemData"]:
    for path in results["ItemData"]:
        print(f"   {path}")
else:
    print("   Not found")

print("\n2. ItemsCollection class definition:")
if results["ItemsCollection"]:
    for path in results["ItemsCollection"]:
        print(f"   {path}")
else:
    print("   Not found")

print("\n3. Files with max level/level limit logic:")
if results["max_level"]:
    for path in results["max_level"]:
        print(f"   {path}")
else:
    print("   Not found")
