# ModCore

**ModCore** is a dual mod loading system for Unity that enables mod support for your game. It supports both interface-based logic mods and live `MonoBehaviour` mods, with in-editor compilation and reloading.

---

## 🔧 Features

- Supports two mod types:
  - `IMod` interface-based mods
  - `MonoBehaviour` component-style mods
- Live compilation and in-editor hot reloading
- Includes basic tools to load, reload, and disable mods

---

## 📁 Included

- `ModCompiler` (handles compilation)
- `IMod` interface
- Two mod loaders (`IModLoader`, `MonoBehaviourModLoader`)
- A few test mods to experiment with
- Basic folder structure to organize and disable mods easily

---

## 📚 Documentation

Each component (compiler, loaders, etc.) has its own dedicated and simple documentation file explaining its usage.  
You won’t have to guess what anything does.

---

## 🧘 Final Notes

ModCore is made to be easy to understand and extend.  
This is just the core — several other helper scripts and parts are included to support common modding needs.

---