# Goblin Debug Bridge

通过 HTTP API 与运行中的 Godot 游戏交互，读取运行时状态、控制 Tick 流程。

## 前提

`DebugServer` 是挂在 `Engine` 上的 Comp，随 Engine 生命周期。HTTP 服务在 Engine 启动后始终可用（`localhost:9876`），不依赖游戏是否开始。

- 无活跃游戏时：`/status` 可用（frame=0, actors=0），其他状态接口返回 `{"error": "no game active"}`
- 有活跃游戏时：全部接口可用

## 状态读取 API

所有 GET，返回 JSON。

### GET /status — 游戏运行状态

```json
{"paused": false, "rendering_paused": false, "step_count": 0,
 "breakpoint": null, "frame": 1234, "elapsed": 49.36, "actors": 5}
```

### GET /state — 全量快照

包含 frame、elapsed、timescale、所有 Actor 摘要。

### GET /actors — Actor 摘要列表

```json
[{"id": 1, "type": "HERO", "state": "IDLE"},
 {"id": 2, "type": "ENEMY", "state": "MOVE"}]
```

### GET /actor/{id} — 单个 Actor 详情

返回 tags、spatial(position/euler/scale)、state_machine(current/last/delaybreak)、attributes(HP/MAXHP/ATK/MOVESPEED)、flow。

### GET /state_machines — 所有存活 Actor 的状态

### GET /flow/{id} — 指定 Actor 的管线状态

```json
{"active": true, "owner": 1, "timeline": 15, "length": 25, "pipelines": [10030], "doing_count": 2}
```

### GET /attributes/{id} — 指定 Actor 的属性

```json
{"HP": 100, "MAXHP": 100, "MOVESPEED": 300, "ATTACK": 50}
```

带缩放时：
```json
{"HP": {"value": 80, "raw": 100, "scale": 800}}
```

## Tick 控制 API

所有 POST/DELETE，用于控制游戏执行。

### POST /control/pause — 暂停逻辑 Tick

### POST /control/resume — 恢复逻辑 Tick

### POST /control/step?n=3 — 步进 n 个逻辑帧

- `n` 可选，默认 1
- 如果当前暂停，会自动临时恢复执行 n 帧后再次暂停

### POST /control/pause_render — 冻结渲染（逻辑仍在跑）

### POST /control/resume_render — 恢复渲染

### POST /control/breakpoint — 设置断点

**状态变更断点**（当任意 Hero 进入 DEATH 暂停）：
```json
{"type": "StateChange", "actorFilter": "HERO", "targetState": "DEATH"}
```

**属性断点**（当 Actor 3 HP < 50 暂停）：
```json
{"type": "Attribute", "actorFilter": "3", "attrName": "HP", "op": "lt", "value": 50}
```

**帧断点**（到达帧 1000 暂停）：
```json
{"type": "Frame", "targetFrame": 1000}
```

支持的 `op`：`lt`/`le`/`gt`/`ge`/`eq`/`ne`
`actorFilter`：`"*"`（所有）、`"HERO"`（按类型）、`"3"`（按 ID）

### DELETE /control/breakpoint — 清除断点

## 输入注入 API

### POST /input — 模拟输入

```json
{"seat": 1, "type": "BA", "pressed": true}
```

摇杆输入：
```json
{"seat": 1, "type": "JOYSTICK", "pressed": true, "direx": 1000, "direy": 0}
```

支持的类型：`JOYSTICK`/`BA`/`BB`/`BC`
`direx`/`direy` 值域 `[-1000, 1000]`

输入在**下一个逻辑帧**生效，通过 `Stage.PushInput` 走正常输入路径。

## 调试工作流示例

### 排查「释放技能后卡死」

```
1. GET /state_machines → 找到处于 CASTING 的 Actor
2. GET /actor/{id} → 查看 flow 管线状态，确认管线卡在哪一帧
3. GET /status → 确认 frame、paused 状态
4. POST /control/breakpoint
   {"type": "StateChange", "actorFilter": "*", "targetState": "NONE"}
5. POST /control/resume → 放行
   → 断点命中时自动暂停，停在状态异常那一刻
6. GET /actor/{id} → 查看触发 NONE 时的完整上下文
```

### 复现特定时机

```
1. POST /control/breakpoint {"type": "Frame", "targetFrame": 500}
2. POST /control/resume
3. 到达 frame 500 自动暂停
4. POST /control/step?n=1 → 逐帧步进观察
5. GET /actor/{id} → 每帧检查状态变化
```

## Actor 类型

- `HERO` (3) — 玩家角色
- `ENEMY` (6) — 敌人
- `MAGIC` (4) — 魔法体/子弹
- `BUFF` (5) — Buff
- `FLOW` (2) — 管线 Actor
- `STAGE` (1) — Stage 自身（不暴露给 Actor API）

## 状态值

- `NONE` (0) — 无状态
- `BORN` (1) — 出生
- `DEATH` (2) — 死亡（终态）
- `IDLE` (3) — 待机
- `MOVE` (4) — 移动
- `JUMP` (5) — 跳跃
- `FALL` (6) — 下落
- `CASTING` (7) — 施法
- `BEHIT` (8) — 受击
- `ROLL` (9) — 翻滚
