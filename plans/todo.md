# Követelmények

## Minimum követelmény (azaz a teljesítéshez feltétlenül szükséges)

- [x] Elkészül a Go / ASP.NET Core alkalmazás, pingeli a belső szolgáltatásokat
- [x] Lekérdezhető kívülről minden szolgáltatás állapota
- [x] A konfiguráció fájl alapú
- [x] A szolgáltatás Docker konténerben fut
- [x] Legalább a periodikusság szabályozható
- [ ] Szorgalmi időszak végéig személyesen bemutatott eredmény

## Elvárt követelmény (ez szükséges a jó jegy eléréséhez)

- [x] A periodikusság mellett szabályozható a timeout, sikerességi ráta, stb.
- [-] A szolgátatások elérhetősége aggregálható
- [x] A pingelendő végpontokat a rendszer a Kubernetes API-ja segítségével automatikusan felderíti induláskor, a konfigurációt annotációkból veszi
- [ ] Egyszerű webes felületen vizuálisan megtekinthető minden belső szolgáltatás állapota
- [x] Az alkalmazás GitHub-on publikusan elérhető
- [x] GitHub Actions CI pipeline előállítja a Docker konténert, publikálja azt (Docker Hub vagy GitHub Container Registry)

## A jeleshez ill. iMsc pontok megszerzéséhez szükséges

- [ ] Rendszeres beszámoló a munka menetéről és folyamatos, látható előrehaladás
- [x] A pingelés nem csak az egyes service-eket pingeli, hanem a mögöttük álló podokat (ez szükséges a következő ponthoz)
- [x] Horizontálisan skálázott alkalmazás esetén "minimum N darab, vagy M% fusson" jellegű szabály támogatása
- [x] A pingelendő végpontokat a rendszer a Kubernetes API-ja segítségével folyamatosan figyeli, és új szolgáltatás/pod megjelenésekor és változásakor azt is bevonja a pingelendő körébe
- [-] Az alkalmazáshoz készül Helm chart
    - ingressek mit engedek ki
    - image
    - annotációk
    - resources
