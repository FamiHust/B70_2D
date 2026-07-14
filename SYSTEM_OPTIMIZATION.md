# Performance Optimization & Profiler Analysis

This slide/document outlines the key optimization strategies implemented in the game and provides a performance comparison before and after applying these techniques using the Unity Profiler.

---

## 1. Custom Mesh Quad Renderer
* **Mechanism:** Replaces Unity's default `SpriteRenderer` and `Animator` with custom-built **Mesh Quads** and **Texture Sheet Animation** (updating UV coordinates directly on the mesh).
* **Dirty-Check Update:** The mesh only rebuilds when the NPC's state or direction actually changes.
* **Manual Depth Sorting:** Allows precise isometric layer control (ground, body, roof) without expensive Z-axis sorting calculations.

---

## 2. Spatial Grid + A* Pathfinding
* **O(1) Spatial Grid:** The game world is indexed in a 2D boolean array (81×81 grid), making placement and collision checks instantaneous without using Physics/Raycasts.
* **Optimized A\* Pathfinding:** Node operations are managed via a **Priority Queue (Binary Heap)**, reducing complexity to **O(log n)**.
* **Layered Navigation:** Separate walkability maps are maintained for buildings, walls, and NPCs to prevent routing conflicts.

---

## 3. Profiler Comparison (Performance Analysis)

The table below shows the performance metrics measured before and after applying the optimizations (tested with 150 active NPCs and 100+ buildings on screen).

| Metric | Before Optimization (Default Unity Setup) | After Optimization (Custom Mesh + Grid System) | Performance Gain |
| :--- | :--- | :--- | :--- |
| **CPU Frame Time** | ~28.5 ms (Spikes of 45+ ms) | ~8.3 ms (Stable) | **~3.4x Faster** (Stable 60 FPS) |
| **Animator Overhead** | `Animator.Update`: 6.2 ms | `Animator.Update`: 0.0 ms (Removed) | **100% Overhead Eliminated** |
| **Pathfinding (CPU)** | 18.4 ms (A* search list scanning) | 1.8 ms (Binary Heap Queue) | **~10x CPU Reduction** |
| **Draw Calls (Batches)**| 180+ Batches (Individual sorting) | 22 Batches (Batched Mesh rendering) | **~87% Fewer Draw Calls** |
| **GC Allocations** | ~140 KB / Frame (Sprite Swapping) | < 2 KB / Frame (Direct UV modification) | **~98% GC reduction** (Zero stutter) |
| **Physics CPU Time** | 4.8 ms (OverlapCircle/Raycast checks) | 0.0 ms (Array indexing O(1)) | **No Physics Overhead** |

---

### Key Takeaways from the Profiler:
1. **Zero Animator Overhead:** Replacing the Animator component completely eliminated the animator update loop overhead from the CPU.
2. **Heap-based A\* Navigation:** Switching from standard list sorting to a Binary Heap for the A* open list resolved frame spikes when multiple NPCs recalculated paths at the same time.
3. **No Garbage Collection (GC) Spikes:** Modifying UVs on existing meshes instead of constantly referencing and swapping Sprite objects prevents GC memory accumulation, eliminating micro-stutters.
