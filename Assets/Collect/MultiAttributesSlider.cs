using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 属性
[Serializable]
class AAttribute
{
    public string name = "";
    public Sprite image = null;
    public Color color = Color.white;
    public int min = 0;
    public int max = 0;
    public int value = 0;

    [NonSerialized]
    public ValueSlider valueSlider;
}

// 滑块
class ValueSlider : Selectable
{
    MultiAttributesSlider _multiAttributesSlider;
    AAttribute _attribute;

    public void Init(MultiAttributesSlider multiAttributesSlider, AAttribute attribute)
    {
        _multiAttributesSlider = multiAttributesSlider;
        _attribute = attribute;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        _multiAttributesSlider.BeginSlide(_attribute, eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        _multiAttributesSlider.EndSlide();
    }
}

class MultiAttributesSlider : MonoBehaviour
{
    // 总点数
    [SerializeField]
    int _totalValue;

    // 属性数组
    [SerializeField]
    AAttribute[] _attributes;

    //剩余点数
    int _restValue;

    // 一个点数对应的像素大小
    float pixelsPerPoint;

    // 保存滑块按下时的信息
    AAttribute _currentAttribute = null;
    PointerEventData _eventData;
    int _beginValue;
    int _beginRestValue;


    // 当鼠标按下任何一个滑块时调用
    public void BeginSlide(AAttribute currentAttribute, PointerEventData eventData)
    {
        _currentAttribute = currentAttribute;
        _eventData = eventData;
        _beginValue = currentAttribute.value;
        _beginRestValue = _restValue;
    }


    // 当鼠标从任何一个滑块释放时调用
    public void EndSlide()
    {
        _currentAttribute = null;
    }


    // 初始化
    void Awake()
    {
        // 需要通过自定义编辑器来保证 Inspector 填写的参数完全合理。这个例子忽略这一步。

        // 统计已使用的点数
        int valueCount = 0;
        for (int i = 0; i < _attributes.Length; i++)
        {
            valueCount += _attributes[i].value;
        }

        // 计算剩余点数
        _restValue = _totalValue - valueCount;

        RectTransform lastParent = transform as RectTransform;

        // 计算一个点数对应的像素大小
        pixelsPerPoint = lastParent.sizeDelta.x / _totalValue;

        // 创建每个滑块；更好的做法是，在自定义编辑器中使用一个按钮来生成所有滑块
        for (int i = 0; i < _attributes.Length; i++)
        {
            GameObject slider = new GameObject(_attributes[i].name);

            // 初始化 RectTransform
            RectTransform rect = slider.AddComponent<RectTransform>();
            rect.SetParent(lastParent, false);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            if (i == 0)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = new Vector2(0, 1);
            }
            else
            {
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = Vector2.one;
            }
            rect.sizeDelta = new Vector2(pixelsPerPoint * _attributes[i].value, 0);

            // 初始化 Image
            Image image = slider.AddComponent<Image>();
            image.sprite = _attributes[i].image;
            image.color = _attributes[i].color;
            image.type = Image.Type.Sliced;
            image.fillCenter = true;

            // 初始化 ValueSlider
            _attributes[i].valueSlider = slider.AddComponent<ValueSlider>();
            _attributes[i].valueSlider.Init(this, _attributes[i]);

            // 将当前 RectTransform 作为下一个滑块的父级
            lastParent = rect;
        }
    }


    // 更新滑块的值
    void Update()
    {
        if (_currentAttribute != null)
        {
            // 计算滑动距离对应的点数变化
            int deltaValue = Mathf.RoundToInt((_eventData.position.x - _eventData.pressPosition.x) / pixelsPerPoint);

            // 受最小、最大值限制的点数变化
            deltaValue = Mathf.Clamp(_beginValue + deltaValue, _currentAttribute.min, _currentAttribute.max) - _beginValue;

            // 更新剩余点数
            _restValue = _beginRestValue - deltaValue;

            // 如果剩余点数用完，需要减少点数变化
            if (_restValue < 0)
            {
                deltaValue += _restValue;
                _restValue = 0;
            }

            // 更新当前点数
            _currentAttribute.value = _beginValue + deltaValue;

            // 更新滑块大小
            (_currentAttribute.valueSlider.transform as RectTransform).sizeDelta
                = new Vector2(pixelsPerPoint * _currentAttribute.value, 0);
        }
    }
}