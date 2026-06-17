# AutoRating — ADOFAI 自动评级 MOD

## 👥 Credits
- **GalaxySpeaker** — 作者 / 算法 / 开发
- **Sharky** — 技术支持
- **SensenPlayer** — 特别感谢

---

## 📖 使用指南 / User Guide

### 🎮 基本使用

进入任意关卡后，MOD 会自动计算谱面评级并显示在屏幕左上角：

```
Star Rating: 12.34 ★
TUF: G5
GG: 20.2
```

### ⌨️ 快捷键（默认 F9）

在关卡中按 **F9** 可以手动重新计算当前谱面的评级。

> 可在设置中修改快捷键、添加 Ctrl/Alt/Shift 组合键、或完全禁用快捷键。

---

### ⚙️ 设置面板说明

按 `Ctrl+F10` 打开 UnityModManager → 点击 **AutoRating** 进入设置。

| 分组 | 选项 | 说明 |
|------|------|------|
| **Position** | X / Y | 拖动滑块调整 UI 在屏幕上的位置 |
| **Appearance** | Size | 字体大小 (10–100) |
| | Gap | 行间距 |
| | Use Bold | 加粗字体 |
| **Display** | Auto Rating | 关闭后停止评级计算，仅保留 Score 显示 |
| | Score | 显示 Score（基础分 × 倍率 × 连击 × X精度）|
| | Score v2 | 显示 Score v2（同上但不含基础倍率）|
| | PP | 显示 PP / Max PP |
| | Speed / Rhythm / Stamina | 显示各项子评级（Speed → FinalSpeed 等）|
| | Only Show in Gameplay | 只在打歌时显示，选关界面隐藏 |
| **Hotkey** | Enable / Disable | 开关快捷键 |
| | Key: 当前按键 | 显示当前快捷键 |
| | Ctrl / Alt / Shift | 组合键修饰 |
| | Change Hotkey | 按下后按任意键重新绑定 |

---

### 📊 显示项目含义

| 显示 | 含义 |
|------|------|
| **Star Rating** | 谱面综合星级（基于 Speed + Rhythm + Stamina） |
| **★ 颜色** | 0–2 青 → 2–4 蓝 → ... → 20–22 暗紫 → 24+ 黑 |
| **TUF: G5** | TUF 等级标签（P1–P20, G1–G20, U1–U20, PGU100） |
| **GG: 20.2** | GG 评级数字 |
| **Score** | 含基础倍率惩罚的最终得分 |
| **Score v2** | 不含基础倍率的总得分 |
| **PP** | Performance Points（当前 / 最大值） |

---

### 💡 使用技巧

1. **只想看 Score？** 关闭 Auto Rating 开关 + 只留 Score 和 Score v2 的勾选，画面极简
2. **调整 UI 位置** 直接拖动 X/Y 滑块，改动实时生效
3. **所有设置自动保存**，重启游戏不会丢失

---

## 📝 更新日志



### 2023/11/11 ver 0.0.0
### 2024/7 ver 0.0.1
### 2024/12 ver 0.0.2	
### 2025/1/13 ver 0.1.0
### 2025/1/22 ver 0.1.1
### 2025/2/1 ver 0.2.0
### 2025/2/9 ver 0.2.1
### 2025/2/12 ver 0.2.2
### 2025/2/13 ver 0.3.0
### 2025/2/14 ver 0.3.1
### 2025/2/17 ver 0.4.0
### 2025/2/18 ver 0.5.0
### 2025/2/20 ver 0.6.0
### 2025/2/22 ver 0.6.1
### 2025/2/23 ver 0.6.2
### 2025/2/24 ver 0.6.3	
### 2025/3/3 ver 0.7.0
### 2025/3/5 ver 0.8.0
-	修复了json读取的问题
-	又一次修复了json读取的问题
-	修复了NaN问题
-	删去了Overlayer附属
### 2025/3/5 ver 0.8.1
-	**完全修复了旧版json读取的问题** (Perhaps)
### 2025/3/6 ver 0.9.0
-	**AutoRating 与Adofai.gg和TUF Forum的等级估算**
-	调整了Rhythm Rating的部分数值
### 2025/3/6 ver 0.9.1
-	增加了一些颜色
### 2025/3/8 ver 0.9.2
-	增加保存设置功能
-	增加文字自定义间距，字体加粗
-	自定义输入框
### 2025/7/31 ver 0.9.7
- 大修改
### 2026/6/17 ver 1.0.0
- 算法更新与优化
- 适配 ADOFAI 版本
- 修复大量兼容性问题
- 新增独立数据显示开关（每行可单独开关）
- 新增描边字体（任意背景清晰可见）
- 移除 MathNet.Numerics 外部依赖

###算法细则已隐藏 [暂时的]

**Full Changelog**: https://github.com/GalaxySpeaker/AutoRating/commits/AutoRating
