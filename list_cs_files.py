import os

# Find all C# files in the workspace
workspace_root = r"c:\B70_2D"
cs_files = []

for root, dirs, files in os.walk(workspace_root):
    # Skip build directories
    dirs[:] = [d for d in dirs if d not in ['.git', 'node_modules', '.vs', 'Library', 'Temp', 'obj', 'bin']]
    
    for file in files:
        if file.endswith('.cs'):
            filepath = os.path.join(root, file)
            rel_path = filepath.replace(workspace_root, "").lstrip("\\")
            cs_files.append(rel_path)

# Sort and display
cs_files.sort()

print(f"Total C# files found: {len(cs_files)}\n")

# Look specifically for Data-related files and Item-related files
print("=== Data-related files ===")
for f in cs_files:
    if 'data' in f.lower():
        print(f)

print("\n=== Item-related files ===")
for f in cs_files:
    if 'item' in f.lower():
        print(f)

print("\n=== Collection-related files ===")
for f in cs_files:
    if 'collection' in f.lower():
        print(f)

print("\n=== All Scripts files (first 50) ===")
scripts_files = [f for f in cs_files if 'Scripts' in f]
for f in scripts_files[:50]:
    print(f)
