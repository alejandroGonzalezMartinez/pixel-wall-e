# 🎨 Pixel Wall-E

**Pixel Wall-E** es un mini lenguaje de programación diseñado para crear dibujos pixelados usando instrucciones sencillas. El proyecto incluye un intérprete visual hecho con **Blazor WebAssembly**, que permite escribir código y ver cómo se dibuja en un lienzo.

🌐 Puedes probarlo directamente aquí:  
👉 [https://pixel-wall-e-314.vercel.app](https://pixel-wall-e-314.vercel.app)

## 🚀 ¿Qué puedes hacer?

Con Pixel Wall-E puedes:
- Spawnear un pincel en una coordenada.
- Cambiar el color o el tamaño del pincel.
- Dibujar líneas, rectángulos y círculos.
- Rellenar áreas.
- Evaluar condiciones con expresiones y saltos (`Goto`).

## 🧠 Ejemplo de código

```txt
Spawn(10, 20)
Color("green")
Size(3)
DrawLine(0, 1, 5)
DrawCircle(1, 0, 4)
Color("brown")
DrawRectangle(0, 1, 2, 3, 5)