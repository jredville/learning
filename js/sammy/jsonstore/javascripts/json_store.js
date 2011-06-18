(function($) {
    var app = $.sammy('#main', function() {
        this.use('Template');

        this.around(function(callback) {
            var context = this;
            this.load('data/items.json')
                .then(function(items) {
                    context.items = items;
                })
                .then(callback);
        });
        this.get('#/', function(context) {
            context.app.swap('');
            $.each(context.items, function(i, item) {
                context.render('templates/item.template', {id: i, item: item})
                       .appendTo(context.$element());
            });
        });

        this.get('#/item/:id', function(context) {
            context.item = context.items[context.params['id']];
            if (!context.item) return context.notFound();
            context.partial('templates/item_detail.template');
        });
    });

    $(function() {
        app.run('#/');
    });
})(jQuery);
