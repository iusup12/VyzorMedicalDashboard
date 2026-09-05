/* Original Vyzor compiled chunks loader for ASP.NET Core MVC.
   The files are the compiled Vyzor/Next.js assets supplied with the project.
   The Sales page chunk is not included in the supplied archive, so this loader
   loads the available runtime/layout chunks only. */
(function () {
    var base = '/_next/static/chunks/';
    var scripts = [
        'webpack-a586a0176dd7250e.js',
        '4bd1b696-3a30cd4d344059f9.js',
        '1684-8eec5587e563fe4d.js',
        'main-app-f3e687b3f94d7221.js',
        '41ade5dc-1ce412a688519a96.js',
        '4540-942c2e8d1572b137.js',
        '1990-44267b0338e64b0e.js',
        'layout-b9614f655be97b49.js',
        '8522-7a741edffa6c5981.js',
        '5793-67dd30eccf062cab.js',
        '6874-c35fb74e4facf6ee.js',
        '6766-7dc3eeb2f8386c53.js',
        '8054-e7a3c1db37bcd858.js',
        '7997-47080342bb8ae44a.js',
        '4003-d99f59cad33eae1c.js',
        '5453-5cf0578cdf28e3e8.js',
        '8939-84171ab0e5ff134c.js',
        '1085-b4829625dcdba232.js',
        '9949-47ccd093a1a9b412.js',
        '382-fd31cce6b559fcde.js',
        '9534-890f1312142d8c86.js',
        '8000-26a3f9764382f42b.js',
        '932-70714b334728847d.js',
        '13633bf0-dd728620ff28721c.js',
        'c16f53c3-373df21a21640a90.js',
        '6492-f1d56cc2f536f31e.js',
        '3311-487454a3f9751927.js',
        '7435-bb867119c8140857.js',
        '4492-33fb97f3e0be1962.js'
    ];
    function load(i) {
        if (i >= scripts.length) return;
        var s = document.createElement('script');
        s.src = base + scripts[i];
        s.async = false;
        s.onload = function () { load(i + 1); };
        s.onerror = function () { console.warn('Vyzor chunk failed to load:', s.src); load(i + 1); };
        document.head.appendChild(s);
    }
    load(0);
})();
