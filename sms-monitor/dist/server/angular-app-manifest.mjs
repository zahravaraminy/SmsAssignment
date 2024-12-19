
export default {
  bootstrap: () => import('./main.server.mjs').then(m => m.default),
  inlineCriticalCss: true,
  baseHref: '/SmsRateLimiter/',
  locale: undefined,
  routes: [
  {
    "renderMode": 2,
    "route": "/SmsRateLimiter"
  }
],
  assets: {
    'index.csr.html': {size: 522, hash: 'ba2a7146d2ed09ccc92284d77173e68467e74390c284434e584a8bc2e5aa0764', text: () => import('./assets-chunks/index_csr_html.mjs').then(m => m.default)},
    'index.server.html': {size: 1035, hash: '83a018d8b7930518c00a8465e0cd391ac3d560d6db4f13b652faae7f538fd0fc', text: () => import('./assets-chunks/index_server_html.mjs').then(m => m.default)},
    'index.html': {size: 3579, hash: '0f6d51b920a3df769ac197aec76dc0b523f1e8d29fdd9ce0cef7338e9f006b47', text: () => import('./assets-chunks/index_html.mjs').then(m => m.default)},
    'styles-5INURTSO.css': {size: 0, hash: 'menYUTfbRu8', text: () => import('./assets-chunks/styles-5INURTSO_css.mjs').then(m => m.default)}
  },
};
