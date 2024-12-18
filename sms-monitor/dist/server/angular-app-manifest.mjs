
export default {
  bootstrap: () => import('./main.server.mjs').then(m => m.default),
  inlineCriticalCss: true,
  baseHref: '/<SmsRateLimiter>/',
  locale: undefined,
  routes: [
  {
    "renderMode": 2,
    "route": "/<SmsRateLimiter>"
  }
],
  assets: {
    'index.csr.html': {size: 524, hash: '18777a73b089ac81b1a3f8a4e6d15a9ee873c7d136c06bb7cbfe06af0a48ab2b', text: () => import('./assets-chunks/index_csr_html.mjs').then(m => m.default)},
    'index.server.html': {size: 1037, hash: 'd938d07e707e8b20a55e4ae320b2f3e087254369072d425b528990d41468c9fb', text: () => import('./assets-chunks/index_server_html.mjs').then(m => m.default)},
    'index.html': {size: 3607, hash: '79ed6b47eef30b6094e800ffda4c7f056fced5716176519919a66832fb2ee359', text: () => import('./assets-chunks/index_html.mjs').then(m => m.default)},
    'styles-5INURTSO.css': {size: 0, hash: 'menYUTfbRu8', text: () => import('./assets-chunks/styles-5INURTSO_css.mjs').then(m => m.default)}
  },
};
