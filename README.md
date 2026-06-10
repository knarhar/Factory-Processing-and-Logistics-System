# Factory Processing & Logistics System

A console-based factory pipeline simulation built in C# (.NET 8), where items are produced by machines, inspected for quality, stored, and transported to final stock — all driven by a tick-based engine with configurable parameters.

## Overview

The simulation models a real-world factory pipeline across six stages:

```
Machines → Order Line → Quality Checker → Storage → Transport System → Stock
```

Each stage runs synchronously within a single simulation tick. The system is fully configurable at startup and stops automatically when all items have been produced and processed through the entire pipeline.

## Components

### 1. Machines (Producers)
- Three machines run in parallel, each producing a specific item type: **A (Widget)**, **B (Gadget)**, **C (Device)**
- Each machine has a configurable production interval (in ticks) and a total item count limit
- Items are assigned globally unique sequential IDs using a thread-safe shared counter (`Interlocked.Increment`)

### 2. Order Line (Input Buffer)
- A bounded FIFO queue that sits between machines and the quality checker
- Has a fixed capacity; overflow items go into an unbounded resizable waitlist queue
- When a slot opens up (on dequeue), the waitlist drains automatically to keep the main queue full
- Thread-safe via `lock`

### 3. Quality Checker (Processor)
- Pulls one item at a time from the order line
- Processes each item for a random number of ticks (between configured min and max)
- Uses a seeded `Random` instance for reproducible results
- Passes items to Storage or discards them based on a configurable quality percentage
- Logs every pickup, pass, and fail with colored output

### 4. Storage (Intermediate Warehouse)
- Holds passed items grouped by type on three internal shelves (one per item type)
- Has a fixed capacity across all shelves
- `TakeOldestItem()` scans all shelf fronts and returns the item with the smallest ID, preserving global FIFO order across types
- Thread-safe via `lock`

### 5. Transport System (Loader)
- Arrives periodically (every N ticks)
- Picks up to its capacity of items from Storage using `TakeOldestItem()`
- Delivers them directly to Stock
- Logs arrivals and delivery counts

### 6. Stock (Final Destination)
- Permanent storage for finalized items, grouped by type
- Shares base logic with Storage via the `ItemShelf` abstract base class
- No removal — items only go in

## Architecture

### Custom Data Structures
No built-in collections (`List`, `Dictionary`, `Queue`, etc.) are used. All data structures are hand-implemented:

- **`Queue`** — circular buffer with optional resizing, used for the order line, waitlist, and all item shelves
- **`RollingLog`** — fixed-size circular log that overwrites oldest entries, used for the event history display

### Class Hierarchy
```
ItemShelf (abstract)
├── Storage
└── Stock
```

### Thread Safety
All shared state (`OrderLine`, `Storage`, `Stock`, `SimulationLogger`) uses `lock` for safe concurrent access. The global item ID counter uses `Interlocked.Increment`.

## Configuration

At startup the simulation prompts for all parameters interactively. Press **Enter** to accept the default value for any field.

| Parameter | Default | Description |
|---|---|---|
| Start item ID | 100 | First item ID assigned |
| Order line capacity | 5 | Max items in the main queue |
| Storage capacity | 50 | Max items across all shelves |
| Stock capacity | 200 | Max items in final stock |
| Min quality check ticks | 1 | Minimum processing time |
| Max quality check ticks | 3 | Maximum processing time |
| Quality percentage | 70 | % chance an item passes |
| Random seed | 42 | Seed for reproducible runs |
| Transport arrival interval | 4 | Ticks between transport visits |
| Transport capacity | 6 | Max items per transport trip |
| Machine A interval | 1 | Ticks between A productions |
| Machine B interval | 2 | Ticks between B productions |
| Machine C interval | 3 | Ticks between C productions |
| Machine A count | 1 | Total items A will produce |
| Machine B count | 1 | Total items B will produce |
| Machine C count | 1 | Total items C will produce |

## Simulation Loop

Each tick executes in this order:

1. All machines attempt to produce an item
2. Quality checker processes its current item (or picks up a new one)
3. Transport system checks if it's time to arrive and collect
4. Current state is rendered to the console
5. Completion is checked — if all items are through the pipeline, the simulation stops

Press **Q** at any time to quit early.

---

## Display

The console renders the full simulation state each tick:

```
╔══════════════════════════════════════════════╗
║   Factory Simulation — Tick 12               ║
╚══════════════════════════════════════════════╝

── Machines ──────────────────────────────────
  Machine A    | produced: 10
  Machine B    | produced: 5
  Machine C    | produced: 3

── Order Line ────────────────────────────────
  Queued:   8 / 10
  Overflow: 2

── Quality Checker ───────────────────────────
  Status: Processing item 105 (C) — 1 tick(s) left

── Storage ───────────────────────────────────
  Total:  3
  Type A: 2
  Type B: 1
  Type C: 0

── Stock ─────────────────────────────────────
  Total:  4
  Type A: 2
  Type B: 1
  Type C: 1

── Product Flow ──────────────────────────────
  [Tick 8]  Transport arrived.
  [Tick 8]  Transport delivered 1 item(s) to stock.
  [Tick 9]  QualityChecker picked up item 104 (A), processing for 2 tick(s).
  [Tick 11] Item 104 (A) passed quality check → Storage.
  [Tick 12] QualityChecker picked up item 105 (C), processing for 1 tick(s).
```

Log lines are color-coded:
- **Cyan** — item picked up by quality checker
- **Green** — item passed quality check
- **Red** — item failed quality check
- **Dark Red** — item dropped due stock or storage overflow
- **Yellow** — transport arrived / delivered

## Completion Summary

When all items have been produced and processed through the full pipeline, the simulation prints a final summary:

```
  ✓ All items processed. Simulation complete.

  Machine A — 10 produced,  7 passed,  3 failed
  Machine B — 5 produced,   4 passed,  1 failed
  Machine C — 3 produced,   2 passed,  1 failed

  Total passed: 13
  Total failed: 5
```

## Project Structure

```
Factory Processing and Logistics System/
├── Components/
│   ├── Items/
│   │   ├── Item.cs
│   │   ├── ItemType.cs
│   │   └── ItemStatus.cs
│   ├── Machine.cs
│   ├── OrderLine.cs
│   ├── Queue.cs
│   ├── QualityChecker.cs
│   ├── ItemShelf.cs
│   ├── Storage.cs
│   ├── Stock.cs
│   └── TransportSystem.cs
├── Core/
│   └── Config.cs
├── Rendering/
│   ├── RollingLog.cs
│   └── SimulationLogger.cs
├── MainSimulation/
│   └── Simulation.cs
└── Program.cs
```

## Key Design Decisions

- **Tick-based engine** — simple, deterministic, easy to reason about and debug
- **No generics or built-in collections** — all data structures implemented from scratch
- **Seeded random** — same seed produces identical runs, useful for debugging
- **Global item IDs** — `Interlocked.Increment` on a static counter ensures unique IDs across all machines regardless of concurrency
- **FIFO across types** — storage preserves global arrival order by comparing item IDs across shelf fronts on every pick
- **Separation of concerns** — each component owns only its own logic; logging and rendering live entirely in `SimulationLogger`
