Swarm
=====

**Swarm** is an experimental project that tries to find an interesting way of
utilizing the [procedural instancing] feature that was newly introduced in
Unity 5.6.

There are two types of renderers in *Swarm*.

**Swirling Swarm**

![screenshot](http://i.imgur.com/qyKgqUg.gif)
![screenshot](http://i.imgur.com/hNgbg86.gif)

*Swirling Swarm* simulates particle motion within a [divergence-free noise
field] and draws trace lines along it. The simulation is reset in every frame,
but the noise field keeps moving slowly, so that it gives the impression that
the swirls are slowly moving around and changing their shapes.

**Crawling Swarm**

![screenshot](http://i.imgur.com/J9XxrgG.gif)
![screenshot](http://i.imgur.com/sZGZsXR.gif)

The basic concept of *Crawling Swarm* is the same to *Swirling Swarm*; it
simulates particle motion within a noise field, but it's constrained with a
[distance field volume]. It tries to keep distances between the particles and
the object surface as low as possible. It gives the feeling that the lines are
crawling around on the surface and cover it.

[procedural instancing]: https://docs.unity3d.com/ScriptReference/Graphics.DrawMeshInstancedIndirect.html
[divergence-free noise field]: http://martian-labs.com/martiantoolz/files/DFnoiseR.pdf
[distance field volume]: https://github.com/keijiro/DFVolume

System Requirements
-------------------

- Unity 5.6 or later

*Swarm* only runs on the platforms that support [compute shaders] and [GPU
instancing].

[compute shaders]: https://docs.unity3d.com/Manual/ComputeShaders.html
[GPU instancing]: https://docs.unity3d.com/Manual/GPUInstancing.html

License
-------

[MIT](LICENSE.md)
