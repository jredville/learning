{{#items}}
    <div class="item">
        <div class="item-image">
            <a href="#/item/{{id}}">
                <img src="{{image}}" alt="{{title}}" />
            </a>
        </div>
        <div class="item-artist">{{artist}}</div>
        <div class="item-title">{{title}}</div>
        <div class="item-price">{{price}}</div>
    </div>
{{/items}}
